using System.Linq;
using System.Web.Mvc;
using MaSV_MyStore.Models;
using MaSV_MyStore.Models.ViewModel;
using PagedList;

namespace MaSV_MyStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        private CartService GetCartService() { return new CartService(Session); }

        // Giỏ hàng nâng cao: nhóm theo danh mục + sản phẩm tương tự
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();
            var catIds = cart.Items.Select(i => i.CategoryID).Distinct().ToList();
            var inCart = cart.Items.Select(i => i.ProductID).ToList();
            cart.SimilarProducts = db.Products
                .Where(p => catIds.Contains(p.CategoryID) && !inCart.Contains(p.ProductID))
                .OrderByDescending(p => p.SoldCount).ToPagedList(page ?? 1, 6);
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity = 1, string returnUrl = null)
        {
            var product = db.Products.Include("Category").SingleOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                var svc = GetCartService();
                var cart = svc.GetCart();
                cart.AddItem(product, quantity < 1 ? 1 : quantity);
                svc.SaveCart(cart);
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var svc = GetCartService();
            var cart = svc.GetCart();
            cart.RemoveItem(id);
            svc.SaveCart(cart);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var svc = GetCartService();
            var cart = svc.GetCart();
            cart.UpdateQuantity(id, quantity);
            svc.SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Tick chọn mua / bỏ chọn
        [HttpPost]
        public ActionResult ToggleSelect(int id, bool selected)
        {
            var svc = GetCartService();
            var cart = svc.GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductID == id);
            if (item != null) item.Selected = selected;
            svc.SaveCart(cart);
            return Json(new { total = cart.TotalValue.ToString("N0") + " VND" });
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
