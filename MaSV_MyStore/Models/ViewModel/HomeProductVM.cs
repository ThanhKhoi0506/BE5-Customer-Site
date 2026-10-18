using System.Collections.Generic;
using PagedList;

namespace MaSV_MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        public List<Product> FeaturedProducts { get; set; }
        public IPagedList<Product> NewProducts { get; set; }
        public string SearchTerm { get; set; }
    }

    public class ProductListVM
    {
        public Category Category { get; set; }
        public IPagedList<Product> Products { get; set; }
    }
}
