using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaSV_MyStore.Models
{
    [Table("Customer")]
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Username { get; set; }
        [ForeignKey("Username")]
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
