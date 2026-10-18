using System.Data.Entity;

namespace MaSV_MyStore.Models
{
    // DbContext (Code First trên CSDL MyStore có sẵn - không tạo/migrate DB)
    public class MyStoreEntities : DbContext
    {
        public MyStoreEntities() : base("name=MyStoreEntities")
        {
            Database.SetInitializer<MyStoreEntities>(null);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
