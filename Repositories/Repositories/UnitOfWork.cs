using Interface.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly WebDemoDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        private IDbContextTransaction? _tx;

        // Lazy repositories
        private readonly Lazy<ICartRepository> _cartRepository;
        private readonly Lazy<ICartItemRepository> _cartItemRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<ICustomerRepository> _customerRepository;
        private readonly Lazy<IDiscountRepository> _discountRepository;
        private readonly Lazy<IInventoryRepository> _inventoryRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IOrderDetailRepository> _orderDetailRepository;
        private readonly Lazy<IPaymentRepository> _paymentRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<IReviewRepository> _reviewRepository;
        private readonly Lazy<IShippingRepository> _shippingRepository;
        private readonly Lazy<IAuditLogRepository> _auditLogRepository;
        private readonly Lazy<IWishlistRepositorty> _wishlistRepositorty;
        private readonly Lazy<IWishlistItemRepository> _wishlistItemRepository;
        private readonly Lazy<IProductCategoryRepository> _productCategoryRepository;

        public UnitOfWork(WebDemoDbContext context, ILogger<UnitOfWork>? logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? (ILogger<UnitOfWork>)NullLogger.Instance;

            _cartRepository = new(() => new CartRepository(_context));
            _cartItemRepository = new(() => new CartItemRepository(_context));
            _categoryRepository = new(() => new CategoryRepository(_context));
            _customerRepository = new(() => new CustomerRepository(_context));
            _discountRepository = new(() => new DiscountRepository(_context));
            _inventoryRepository = new(() => new InventoryRepository(_context));
            _orderRepository = new(() => new OrderRepository(_context));
            _orderDetailRepository = new(() => new OrderDetailRepository(_context));
            _paymentRepository = new(() => new PaymentRepository(_context));
            _productRepository = new(() => new ProductRepository(_context));
            _reviewRepository = new(() => new ReviewRepository(_context));
            _shippingRepository = new(() => new ShippingRepository(_context));
            _wishlistRepositorty = new(() => new WishlistRepositorty(_context));
            _wishlistItemRepository = new(() => new WishlistItemRepository(_context));
            _productCategoryRepository = new(() => new ProductCategoryRepository(_context));
            _auditLogRepository = new(() => new AuditLogRepository(_context));
        }

        // Repositories
        public ICartRepository CartRepository => _cartRepository.Value;
        public ICartItemRepository CartItemRepository => _cartItemRepository.Value;
        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public ICustomerRepository CustomerRepository => _customerRepository.Value;
        public IDiscountRepository DiscountRepository => _discountRepository.Value;
        public IInventoryRepository InventoryRepository => _inventoryRepository.Value;
        public IOrderRepository OrderRepository => _orderRepository.Value;
        public IOrderDetailRepository OrderDetailRepository => _orderDetailRepository.Value;
        public IPaymentRepository PaymentRepository => _paymentRepository.Value;
        public IProductRepository ProductRepository => _productRepository.Value;
        public IReviewRepository ReviewRepository => _reviewRepository.Value;
        public IShippingRepository ShippingRepository => _shippingRepository.Value;
        public IWishlistRepositorty WishlistRepositorty => _wishlistRepositorty.Value;
        public IWishlistItemRepository WishlistItemRepository => _wishlistItemRepository.Value;
        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository.Value;
        public IAuditLogRepository AuditLogRepository => _auditLogRepository.Value;

        public bool HasActiveTransaction => _tx != null;

        // ---- Transactions (sync) ----
        public void BeginTransaction()
        {
            if (_tx != null) return; // already active
            try
            {
                _tx = _context.Database.BeginTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to begin transaction");
                throw;
            }
        }

        public void Commit()
        {
            if (_tx == null) return;
            try
            {
                _tx.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to commit transaction");
                throw;
            }
            finally
            {
                _tx.Dispose();
                _tx = null;
            }
        }

        public void Rollback()
        {
            if (_tx == null) return;
            try
            {
                _tx.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Rollback threw");
            }
            finally
            {
                _tx.Dispose();
                _tx = null;
            }
        }

        // ---- SaveChanges ----
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error on SaveChanges");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdate error on SaveChanges");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error on SaveChanges");
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error on SaveChangesAsync");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DbUpdate error on SaveChangesAsync");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error on SaveChangesAsync");
                throw;
            }
        }
        public void ConfigureReadOnly() =>
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        public IDisposable WithAutoDetectChanges(bool enable)
        {
            var prev = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = enable;
            return new Restore(() => _context.ChangeTracker.AutoDetectChangesEnabled = prev);
        }

        private sealed class Restore : IDisposable
        {
            private readonly Action _a;
            public Restore(Action a) => _a = a;
            public void Dispose() => _a();
        }

        // ---- Dispose ----
        public void Dispose()
        {
            try
            {
                if (_tx != null)
                {
                    try { _tx.Rollback(); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Rollback in Dispose failed"); }
                    _tx.Dispose();
                    _tx = null;
                }
            }
            finally
            {
                _context.Dispose();
            }
        }
        public void Configuration()
        {
            _context.ConfigureAwait(false);
        }
    }
}