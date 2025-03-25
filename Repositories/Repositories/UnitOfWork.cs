using Interface.IRepositories;
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
        private readonly Lazy<IWishlistRepositorty> _wishlistRepositorty;
        private readonly Lazy<IWishlistItemRepository> _wishlistItemRepository;
        private IDbContextTransaction _transaction;

        private readonly Lazy<ICartRepository> _cartRepository;
        private readonly Lazy<IProductCategoryRepository> _productCategoryRepository;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(WebDemoDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _cartRepository = new Lazy<ICartRepository>(() => new CartRepository(_context));
            _cartItemRepository = new Lazy<ICartItemRepository>(() => new CartItemRepository(_context));
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
            _customerRepository = new Lazy<ICustomerRepository>(() => new CustomerRepository(_context));
            _discountRepository = new Lazy<IDiscountRepository>(() => new DiscountRepository(_context));
            _inventoryRepository = new Lazy<IInventoryRepository>(() => new InventoryRepository(_context));
            _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(_context));
            _orderDetailRepository = new Lazy<IOrderDetailRepository>(() => new OrderDetailRepository(_context));
            _paymentRepository = new Lazy<IPaymentRepository>(() => new PaymentRepository(_context));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            _reviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(_context));
            _shippingRepository = new Lazy<IShippingRepository>(() => new ShippingRepository(_context));
            _wishlistRepositorty = new Lazy<IWishlistRepositorty>(() => new WishlistRepositorty(_context));
            _wishlistItemRepository = new Lazy<IWishlistItemRepository>(() => new WishlistItemRepository(_context));
            _productCategoryRepository = new Lazy<IProductCategoryRepository>(() => new ProductCategoryRepository(_context));

        }
        public ICartItemRepository CartItemRepository => _cartItemRepository.Value;
        public ICartRepository CartRepository => _cartRepository.Value;
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

        public void BeginTransaction()
        {
            try
            {
                _transaction = _context.Database.BeginTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to begin transaction");
                throw;
            }
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
                _transaction?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to commit transaction");
                throw;
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DbContext error");
                throw;
            }

        }

        public void Dispose()
        {
            try
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rollback transaction");
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DbContext error");
                throw;
            }

        }
        public void Configuration()
        {
            _context.ConfigureAwait(false);
        }
    }
}