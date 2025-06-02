using Dto;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repositories.Configurations;

namespace Repositories.Repositories
{
    public class WebDemoDbContext : IdentityDbContext<ApplicationUser>
    {
        public WebDemoDbContext(DbContextOptions<WebDemoDbContext> options)
            : base(options)
        {
        }
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet <OrderDetail> OrderDetail { get; set; } = null!;
        public DbSet <CartItem> CartItem { get; set; } = null!;
        public DbSet  <Customer> Customer { get; set; } = null!;
        public DbSet <Discount> Discount { get; set; } = null!;
        public DbSet <Inventory> Inventory { get; set; } = null!;
        public DbSet <Payment> payment { get; set; } = null!;
        public DbSet <Product> Product { get; set; } = null!;
        public DbSet <Review> Review { get; set; } = null!;
        public DbSet <Shipping> Shipping { get; set; } = null!;
        public DbSet<Wishlist>Wishlist { get; set; } = null!;
        public DbSet <WishlistItem> WishlistItem { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistItemConfiguration());
            base.OnModelCreating(modelBuilder);

        }
    }
}