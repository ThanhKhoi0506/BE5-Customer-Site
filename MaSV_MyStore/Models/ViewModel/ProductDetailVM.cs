using System.Collections.Generic;
using PagedList;

namespace MaSV_MyStore.Models.ViewModel
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public IPagedList<Product> TopProducts { get; set; }
    }
}
