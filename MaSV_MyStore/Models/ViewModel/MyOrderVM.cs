using System.Collections.Generic;

namespace MaSV_MyStore.Models.ViewModel
{
    public class MyOrderVM
    {
        public List<Order> Orders { get; set; }
        public string Status { get; set; }
        public string Search { get; set; }
    }
}
