using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MaSV_MyStore.Models;
using MaSV_MyStore.Models.ViewModel;
using PagedList;

namespace MaSV_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        // Menu danh mục (partial)
        [ChildActionOnly]
        public ActionResult CategoriesMenu()
        {
            return PartialView("_CategoriesPV", db.Categories.OrderBy(c => c.CategoryName).ToList());
        }

        // Trang chủ: tìm kiếm + sản phẩm nổi bật + sản phẩm mới (phân trang 6 sp/trang)
        public ActionResult Index(string searchTerm, int? page)
        {
            var q = db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.Trim();
                q = q.Where(p => p.ProductName.Contains(s)
                              || p.ProductDescription.Contains(s)
                              || p.Category.CategoryName.Contains(s));
            }
            int pageNumber = page ?? 1;
            var vm = new HomeProductVM
            {
                SearchTerm = searchTerm,
                FeaturedProducts = q.OrderByDescending(p => p.SoldCount).Take(10).ToList(),
                NewProducts = q.OrderBy(p => p.SoldCount).ThenBy(p => p.ProductID).Take(20).ToPagedList(pageNumber, 6)
            };
            return View(vm);
        }

        // Danh sách sản phẩm theo danh mục
        public ActionResult ProductList(int id, int? page)
        {
            var cat = db.Categories.Find(id);
            if (cat == null) return HttpNotFound();
            var vm = new ProductListVM
            {
                Category = cat,
                Products = db.Products.Where(p => p.CategoryID == id).OrderBy(p => p.ProductName).ToPagedList(page ?? 1, 6)
            };
            return View(vm);
        }

        // Chi tiết sản phẩm
        [HttpGet]
        public ActionResult ProductDetail(int id, int? quantity, int? page)
        {
            var product = db.Products.Include("Category").SingleOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            var vm = new ProductDetailVM
            {
                Product = product,
                Quantity = quantity.HasValue && quantity.Value > 0 ? quantity.Value : 1,
                RelatedProducts = db.Products.Where(p => p.CategoryID == product.CategoryID && p.ProductID != id).Take(8).ToList(),
                TopProducts = db.Products.Where(p => p.CategoryID == product.CategoryID)
                                .OrderByDescending(p => p.SoldCount).Take(8).ToPagedList(page ?? 1, 3)
            };
            return View(vm);
        }

        // Thêm vào giỏ / Mua ngay từ trang chi tiết
        [HttpPost]
        public ActionResult ProductDetail(int id, int quantity, string buyNow)
        {
            var product = db.Products.Include("Category").SingleOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            var svc = new CartService(Session);
            var cart = svc.GetCart();
            cart.AddItem(product, quantity < 1 ? 1 : quantity);
            svc.SaveCart(cart);
            if (!string.IsNullOrEmpty(buyNow)) return RedirectToAction("Checkout", "Order");
            return RedirectToAction("Index", "Cart");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
