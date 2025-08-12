using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.IRepositories
{
    public interface IUnitOfWork
    {
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IDiscountRepository DiscountRepository { get; }
        IInventoryRepository InventoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IProductRepository ProductRepository { get; }
        IReviewRepository ReviewRepository { get; }
        IShippingRepository ShippingRepository { get; }
        IWishlistRepositorty WishlistRepositorty { get; }
         IAuditLogRepository AuditLogRepository { get; }
        IProductCategoryRepository ProductCategoryRepository { get; }
        IWishlistItemRepository WishlistItemRepository { get; }

      

        void BeginTransaction();
        void Commit();
        void Rollback();
        void SaveChanges();
        Task SaveChangesAsync();
        void Configuration();
        void Dispose();
        IDisposable WithAutoDetectChanges(bool enable);
        void ConfigureReadOnly();
    }
}
