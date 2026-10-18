using System.Web;

namespace MaSV_MyStore.Models.ViewModel
{
    // Tương tác giỏ hàng lưu trong Session
    public class CartService
    {
        private readonly HttpSessionStateBase session;
        private const string Key = "Cart";
        public CartService(HttpSessionStateBase session) { this.session = session; }

        public Cart GetCart()
        {
            var cart = session[Key] as Cart;
            if (cart == null) { cart = new Cart(); session[Key] = cart; }
            return cart;
        }
        public void SaveCart(Cart cart) { session[Key] = cart; }
        public void ClearCart() { session[Key] = null; }
    }
}
