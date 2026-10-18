using System.Collections.Generic;
using System.Linq;

namespace MaSV_MyStore.Models.ViewModel
{
    public class Cart
    {
        private readonly List<CartItem> items = new List<CartItem>();
        public List<CartItem> Items { get { return items; } }

        public void AddItem(Product p, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == p.ProductID);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductImage = p.ProductImage,
                    UnitPrice = p.Price,
                    Quantity = quantity,
                    CategoryID = p.CategoryID,
                    Category = p.Category != null ? p.Category.CategoryName : "",
                    Selected = true
                });
            }
            else item.Quantity += quantity;
        }
        public void RemoveItem(int productId) { items.RemoveAll(i => i.ProductID == productId); }
        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item == null) return;
            if (quantity <= 0) RemoveItem(productId); else item.Quantity = quantity;
        }
        public void Clear() { items.Clear(); }

        public int TotalQuantity { get { return items.Sum(i => i.Quantity); } }
        public decimal TotalValue { get { return items.Where(i => i.Selected).Sum(i => i.TotalPrice); } }

        // nhóm theo danh mục (giỏ hàng nâng cao)
        public List<IGrouping<string, CartItem>> GroupedItems { get { return items.GroupBy(i => i.Category).ToList(); } }
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }
    }
}
