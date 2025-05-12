using AutoMapper;
using Dto;
using Interface.IRepositories;
using Moq;
using Service.QueriesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Unit_Test.test
{
    public class ProductQueriesServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductQueriesService _service;

        public ProductQueriesServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _service = new ProductQueriesService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void FindAll_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test Description",
                    Price = 10.0m,
                    Stock = 100,
                    ImageUrl = "http://example.com/image.jpg"
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test Description",
                    Price = 10.0m,
                    Stock = 100,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = false,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindAll()).Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<ProductModel>>(products)).Returns(mapped);

            var result = _service.FindAll();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void FindByCondition_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test Description",
                    Price = 10.0m,
                    Stock = 100,
                    ImageUrl = "http://example.com/image.jpg"
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test Description",
                    Price = 10.0m,
                    Stock = 100,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = false,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<ProductModel>>(products)).Returns(mapped);

            var result = _service.FindByCondition(p => p.Id == 1);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void Get_ShouldReturnMappedProduct()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Test",
                Description = "Test Description",
                Price = 10.0m,
                Stock = 100,
                ImageUrl = "http://example.com/image.jpg"
            };
            var mapped = new ProductModel
            {
                Id = 1,
                Name = "Test",
                Description = "Test Description",
                Price = 10.0m,
                Stock = 100,
                ImagePath = "http://example.com/image.jpg",
                IsFeatured = false,
                ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(new List<Product> { product }.AsQueryable());
            _mapperMock.Setup(m => m.Map<ProductModel>(product)).Returns(mapped);

            var result = _service.Get(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetProductsByCategory_ShouldReturnMappedProducts()
        {
            var productCategoryList = new List<ProductCategory>
            {
                new ProductCategory
                {
                    Product = new Product
                    {
                        Id = 1,
                        Name = "P1",
                        Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'
                        Price = 100.0m, // Fix for CS9035: Required member 'Product.Price'
                        Stock = 50, // Fix for CS9035: Required member 'Product.Stock'
                        ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                    },
                    CategoryId = 1
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Sample Name",
                    Description = "Sample Description",
                    Price = 100.0m,
                    Stock = 10,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = false,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductCategoryRepository.FindByCondition(It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .Returns(productCategoryList.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(It.IsAny<IEnumerable<Product>>())).Returns(mapped);

            var result = _service.GetProductsByCategory(1);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void SearchProducts_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Phone",
                    Description = "Test",
                    Price = 100.0m, // Fix for CS9035: Required member 'Product.Price'
                    Stock = 10, // Fix for CS9035: Required member 'Product.Stock'
                    ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Phone", // Fix for CS9035: Required member 'ProductModel.Name'
                    Description = "Test", // Fix for CS9035: Required member 'ProductModel.Description'
                    Price = 100.0m, // Fix for CS9035: Required member 'ProductModel.Price'
                    Stock = 10, // Fix for CS9035: Required member 'ProductModel.Stock'
                    ImagePath = "http://example.com/image.jpg", // Fix for CS9035: Required member 'ProductModel.ImagePath'
                    IsFeatured = false, // Fix for CS9035: Required member 'ProductModel.IsFeatured'
                    ImageFile = null // Fix for CS9035: Required member 'ProductModel.ImageFile'
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(products)).Returns(mapped);

            var result = _service.SearchProducts("Phone");

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void GetFeaturedProducts_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Sample Name", // Fix for CS9035: Required member 'Product.Name'
                    Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'
                    Price = 100.0m, // Fix for CS9035: Required member 'Product.Price'
                    Stock = 10, // Fix for CS9035: Required member 'Product.Stock'
                    ImageUrl = "http://example.com/image.jpg", // Fix for CS9035: Required member 'Product.ImageUrl'
                    IsFeatured = true
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Sample Name",
                    Description = "Sample Description",
                    Price = 100.0m,
                    Stock = 10,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = true,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(products)).Returns(mapped);

            var result = _service.GetFeaturedProducts();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void GetProducts_ShouldReturnPagedMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Description = "Description 1",
                    Price = 10.0m,
                    Stock = 5,
                    ImageUrl = "http://example.com/image1.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Description = "Description 2",
                    Price = 20.0m,
                    Stock = 10,
                    ImageUrl = "http://example.com/image2.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3",
                    Description = "Description 3",
                    Price = 30.0m,
                    Stock = 15,
                    ImageUrl = "http://example.com/image3.jpg"
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 2,
                    Name = "Sample Name", // Required member 'ProductModel.Name'
                    Description = "Sample Description", // Required member 'ProductModel.Description'
                    Price = 20.0m, // Required member 'ProductModel.Price'
                    Stock = 10, // Required member 'ProductModel.Stock'
                    ImagePath = "http://example.com/image2.jpg", // Required member 'ProductModel.ImagePath'
                    IsFeatured = false, // Required member 'ProductModel.IsFeatured'
                    ImageFile = null // Required member 'ProductModel.ImageFile', assuming null for simplicity
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAll()).Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(It.IsAny<List<Product>>())).Returns(mapped);

            var result = _service.GetProducts(1, 1);

            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void GetProductsByPriceRange_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Sample Name", // Fix for CS9035: Required member 'Product.Name'
                    Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'
                    Price = 10.0m, // Already set
                    Stock = 5, // Fix for CS9035: Required member 'Product.Stock'
                    ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Sample Name",
                    Description = "Sample Description",
                    Price = 10.0m,
                    Stock = 5,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = false,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(products)).Returns(mapped);

            var result = _service.GetProductsByPriceRange(5, 20);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void GetRecentlyAddedProducts_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test", // Fix for CS9035: Required member 'Product.Name'
                    Description = "Test Description", // Fix for CS9035: Required member 'Product.Description'
                    Price = 10.0m,
                    Stock = 100,
                    ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                }
            };
            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 2,
                    Name = "Sample Name",
                    Description = "Sample Description",
                    Price = 200.0m,
                    Stock = 15,
                    ImagePath = "http://example.com/image.jpg",
                    IsFeatured = true,
                    ImageFile = null // Assuming null for simplicity; replace with a valid IFormFile if needed
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAll()).Returns(products.AsQueryable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductModel>>(It.IsAny<List<Product>>())).Returns(mapped);

            var result = _service.GetRecentlyAddedProducts();

            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void GetRelatedProducts_ShouldReturnMappedProducts_WhenProductExists()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Sample Name", // Fix for CS9035: Required member 'Product.Name'
                Description = "Sample Description", // Fix for CS9035: Required member 'Product.Description'
                Price = 10.0m, // Assuming a valid price
                Stock = 5, // Fix for CS9035: Required member 'Product.Stock'
                ImageUrl = "http://example.com/image.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
            };

            var relatedProducts = new List<Product>
            {
                new Product
                {
                    Id = 2,
                    Name = "Product 2", // Fix for CS9035: Required member 'Product.Name'
                    Description = "Description 2", // Fix for CS9035: Required member 'Product.Description'
                    Price = 20.0m,
                    Stock = 10,
                    ImageUrl = "http://example.com/image2.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                },
                new Product
                {
                    Id = 3,
                    Name = "Product 3", // Fix for CS9035: Required member 'Product.Name'
                    Description = "Description 3", // Fix for CS9035: Required member 'Product.Description'
                    Price = 30.0m,
                    Stock = 15,
                    ImageUrl = "http://example.com/image3.jpg" // Fix for CS9035: Required member 'Product.ImageUrl'
                }
            };

            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 2,
                    Name = "Product 2", // Fix for CS9035: Required member 'ProductModel.Name'
                    Description = "Description 2", // Fix for CS9035: Required member 'ProductModel.Description'
                    Price = 20.0m, // Fix for CS9035: Required member 'ProductModel.Price'
                    Stock = 10, // Fix for CS9035: Required member 'ProductModel.Stock'
                    ImagePath = "http://example.com/image2.jpg", // Fix for CS9035: Required member 'ProductModel.ImagePath'
                    IsFeatured = false, // Fix for CS9035: Required member 'ProductModel.IsFeatured'
                    ImageFile = null // Fix for CS9035: Required member 'ProductModel.ImageFile', assuming null for simplicity
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(new List<Product> { product }.AsQueryable());

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(relatedProducts.AsQueryable());

            _mapperMock.Setup(m => m.Map<List<ProductModel>>(relatedProducts)).Returns(mapped);

            var result = _service.GetRelatedProducts(1);

            Assert.Single(result);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void GetRelatedProducts_ShouldReturnEmpty_WhenProductNotFound()
        {
            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(Enumerable.Empty<Product>().AsQueryable());

            var result = _service.GetRelatedProducts(99);

            Assert.Empty(result);
        }
    }
}