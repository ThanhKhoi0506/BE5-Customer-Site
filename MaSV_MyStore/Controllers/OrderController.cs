using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MaSV_MyStore.Models;
using MaSV_MyStore.Models.ViewModel;

namespace MaSV_MyStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        public static readonly string[] Statuses = { "Chờ thanh toán", "Đang xử lý", "Đang vận chuyển", "Đã giao", "Đã hủy" };

        private static decimal ShippingFee(string method) { return method == "Giao nhanh" ? 26000 : 20000; }

        private CheckoutVM BuildVM(Customer customer, List<CartItem> items, string delivery, string payment, string address)
        {
            var vm = new CheckoutVM
            {
                CartItems = items,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                ShippingAddress = string.IsNullOrWhiteSpace(address) ? customer.CustomerAddress : address,
                DeliveryMethod = delivery ?? "Giao nhanh",
                PaymentMethod = payment ?? "COD"
            };
            vm.SubTotal = items.Sum(i => i.TotalPrice);
            vm.ShippingFee = ShippingFee(vm.DeliveryMethod);
            vm.TotalAmount = vm.SubTotal + vm.ShippingFee;
            return vm;
        }

        // ---------- Thanh toán ----------
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = new CartService(Session).GetCart();
            var selected = cart.Items.Where(i => i.Selected).ToList();
            if (selected.Count == 0) return RedirectToAction("Index", "Home");

            var customer = db.Customers.SingleOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null) return RedirectToAction("Login", "Account");
            return View(BuildVM(customer, selected, null, null, null));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var svc = new CartService(Session);
            var cart = svc.GetCart();
            var selected = cart.Items.Where(i => i.Selected).ToList();
            if (selected.Count == 0) return RedirectToAction("Index", "Home");

            var customer = db.Customers.SingleOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(BuildVM(customer, selected, model.DeliveryMethod, model.PaymentMethod, model.ShippingAddress));

            string paymentStatus = "Pending";
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                DeliveryMethod = model.DeliveryMethod,
                ShippingAddress = model.ShippingAddress,
                PaymentStatus = paymentStatus,
                OrderStatus = model.PaymentMethod == "COD" ? "Đang xử lý" : "Chờ thanh toán",
                OrderDetails = new List<OrderDetail>()
            };
            decimal total = 0;
            foreach (var item in selected)
            {
                order.OrderDetails.Add(new OrderDetail { ProductID = item.ProductID, Quantity = item.Quantity, UnitPrice = item.UnitPrice });
                total += item.TotalPrice;
                var p = db.Products.Find(item.ProductID);
                if (p != null) p.SoldCount += item.Quantity;
            }
            order.TotalAmount = total + ShippingFee(model.DeliveryMethod);
            db.Orders.Add(order);
            db.SaveChanges();

            // chỉ xóa các sản phẩm đã mua khỏi giỏ
            foreach (var item in selected) cart.RemoveItem(item.ProductID);
            svc.SaveCart(cart);
            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        // ---------- Xác nhận đơn hàng ----------
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product").Include("Customer")
                          .SingleOrDefault(o => o.OrderID == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // ---------- Đơn hàng của tôi ----------
        public ActionResult MyOrder(string status, string search)
        {
            var q = db.Orders.Include("OrderDetails.Product")
                      .Where(o => o.Customer.Username == User.Identity.Name);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(o => o.OrderStatus == status);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                int oid;
                if (int.TryParse(s, out oid))
                    q = q.Where(o => o.OrderID == oid || o.OrderDetails.Any(d => d.Product.ProductName.Contains(s)));
                else
                    q = q.Where(o => o.OrderDetails.Any(d => d.Product.ProductName.Contains(s)));
            }
            var vm = new MyOrderVM { Orders = q.OrderByDescending(o => o.OrderDate).ToList(), Status = status, Search = search };
            return View(vm);
        }

        // Mua lại: thêm lại các sản phẩm của đơn vào giỏ
        [HttpPost]
        public ActionResult Reorder(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product.Category")
                          .SingleOrDefault(o => o.OrderID == id && o.Customer.Username == User.Identity.Name);
            if (order != null)
            {
                var svc = new CartService(Session);
                var cart = svc.GetCart();
                foreach (var d in order.OrderDetails) cart.AddItem(d.Product, d.Quantity);
                svc.SaveCart(cart);
            }
            return RedirectToAction("Index", "Cart");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
