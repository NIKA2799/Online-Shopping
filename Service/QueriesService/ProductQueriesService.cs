using AutoMapper;
using Interface.IRepositories;
using Interface.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Webdemo.Models;



namespace Service.QueriesService
{
    public class ProductQueriesService : IProductQuery
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductQueriesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<ProductModel> FindAll()
        {
            var model = _unitOfWork.ProductRepository.FindAll();
            var product = _mapper.Map<List<ProductModel>>(model);
            return product;

        }

        public IEnumerable<ProductModel> FindByCondition(Expression<Func<Product, bool>> predicate)
        {
            var model = _unitOfWork.ProductRepository.FindByCondition(predicate);
            var product = _mapper.Map<List<ProductModel>>(model);
            return product;
        }

        public ProductModel Get(int id)
        {
            var model = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == id).SingleOrDefault();
            var product = _mapper.Map<ProductModel>(model);
            return product;
        }
        public IEnumerable<ProductModel> GetProductsByCategory(int categoryId)
        {
            var products = _unitOfWork.ProductCategoryRepository
                .FindByCondition(pc => pc.CategoryId == categoryId)
                .Select(pc => pc.Product)
                .ToList();

            // Map the products to ProductModel
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
        public IEnumerable<ProductModel> SearchProducts(string keyword)
        {
            var products = _unitOfWork.ProductRepository.FindByCondition(p =>
                p.Name.Contains(keyword) || p.Description.Contains(keyword)).ToList();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
        public IEnumerable<ProductModel> GetFeaturedProducts()
        {
            var featuredProducts = _unitOfWork.ProductRepository.FindByCondition(p => p.IsFeatured).ToList();
            return _mapper.Map<IEnumerable<ProductModel>>(featuredProducts);
        }
        public IEnumerable<ProductModel> GetProducts(int pageNumber, int pageSize)
        {
            var products = _unitOfWork.ProductRepository.GetAll()
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
        public IEnumerable<ProductModel> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var products = _unitOfWork.ProductRepository.FindByCondition(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
        public IEnumerable<ProductModel> GetRecentlyAddedProducts()
        {
            var recentProducts = _unitOfWork.ProductRepository.GetAll()
                              .OrderByDescending(p => p.CreateDate)
                              .Take(10)  // Display the 10 most recent products
                              .Take(10).ToList();

            return _mapper.Map<IEnumerable<ProductModel>>(recentProducts);
        }
        public IEnumerable<ProductModel> GetRelatedProducts(int productId, int take = 4)
        {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            var product = _unitOfWork.ProductRepository
                .FindByCondition(p => p.Id == productId)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .SingleOrDefault();
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

            if (product == null)
                return Enumerable.Empty<ProductModel>();

#pragma warning disable CS8604 // Possible null reference argument.
            var categoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
#pragma warning restore CS8604 // Possible null reference argument.
            var minPrice = product.Price * 0.8m;
            var maxPrice = product.Price * 1.2m;

#pragma warning disable CS8604 // Possible null reference argument.
            var related = _unitOfWork.ProductRepository
                .FindByCondition(p =>
                    p.Id != productId &&
                    p.Price >= minPrice && p.Price <= maxPrice &&
                    p.ProductCategories.Any(pc => categoryIds.Contains(pc.CategoryId)) &&
                    p.Stock > 0 && p.IsFeatured)
                .ToList()
                .OrderBy(x => Guid.NewGuid()) // Random sort
                .Take(take)
                .ToList();
#pragma warning restore CS8604 // Possible null reference argument.

            return _mapper.Map<List<ProductModel>>(related);
        }

    }
}
