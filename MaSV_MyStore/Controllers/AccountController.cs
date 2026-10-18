using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using MaSV_MyStore.Models;
using MaSV_MyStore.Models.ViewModel;

namespace MaSV_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        public static string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        // ---------- Menu cá nhân (partial) ----------
        [ChildActionOnly]
        public ActionResult PersonalMenu()
        {
            var cart = new CartService(Session).GetCart();
            var vm = new PersonalMenuVM
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                Username = User.Identity.Name,
                CartCount = cart.TotalQuantity
            };
            return PartialView("_PersonalMenuPV", vm);
        }

        // ---------- Đăng ký ----------
        [HttpGet]
        public ActionResult Register() { return View(new RegisterVM()); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }
            var user = new User { Username = model.Username, Password = Hash(model.Password), UserRole = "Customer" };
            var customer = new Customer
            {
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                CustomerAddress = model.CustomerAddress,
                Username = model.Username
            };
            db.Users.Add(user);
            db.Customers.Add(customer);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // ---------- Đăng nhập ----------
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var hash = Hash(model.Password);
            var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == hash);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }
            Session["Username"] = user.Username;
            FormsAuthentication.SetAuthCookie(user.Username, model.RememberMe);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // ---------- Đăng xuất ----------
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ---------- Thông tin tài khoản ----------
        [Authorize]
        public ActionResult ProfileInfo()
        {
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (c == null) return RedirectToAction("Login");
            var vm = new ProfileInfoVM
            {
                Profile = new UpdateProfileVM
                {
                    Username = c.Username,
                    CustomerName = c.CustomerName,
                    CustomerPhone = c.CustomerPhone,
                    CustomerEmail = c.CustomerEmail,
                    CustomerAddress = c.CustomerAddress
                },
                Password = new ChangePasswordVM()
            };
            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateProfile([Bind(Prefix = "Profile")] UpdateProfileVM model)
        {
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (c == null) return RedirectToAction("Login");
            if (!ModelState.IsValid)
            {
                model.Username = c.Username;
                model.CustomerName = c.CustomerName;
                return View("ProfileInfo", new ProfileInfoVM { Profile = model, Password = new ChangePasswordVM() });
            }
            c.CustomerPhone = model.CustomerPhone;
            c.CustomerEmail = model.CustomerEmail;
            c.CustomerAddress = model.CustomerAddress;
            db.SaveChanges();
            TempData["Msg"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("ProfileInfo");
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Prefix = "Password")] ChangePasswordVM model)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (user == null || c == null) return RedirectToAction("Login");

            if (ModelState.IsValid && user.Password != Hash(model.OldPassword))
                ModelState.AddModelError("Password.OldPassword", "Mật khẩu hiện tại không đúng.");

            if (!ModelState.IsValid)
            {
                var p = new UpdateProfileVM { Username = c.Username, CustomerName = c.CustomerName, CustomerPhone = c.CustomerPhone, CustomerEmail = c.CustomerEmail, CustomerAddress = c.CustomerAddress };
                return View("ProfileInfo", new ProfileInfoVM { Profile = p, Password = new ChangePasswordVM() });
            }
            user.Password = Hash(model.NewPassword);
            db.SaveChanges();
            TempData["Msg"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("ProfileInfo");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
