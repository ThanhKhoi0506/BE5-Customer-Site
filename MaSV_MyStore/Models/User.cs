using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaSV_MyStore.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
