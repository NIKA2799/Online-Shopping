using AutoMapper;
using Dto;
using Interface.IRepositories;
using Microsoft.AspNetCore.Http;
using Moq;
using Org.BouncyCastle.Utilities;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;
using Times = Moq.Times;

namespace Unit_Test.test
{
    public class ProductCommandServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductCommandService _service;

        public ProductCommandServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _service = new ProductCommandService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        // Fix for CS0029: Cannot implicitly convert type 'Microsoft.AspNetCore.Http.IFormFile' to 'string'
        // The issue is that the `ImageFile` property in `ProductModel` is of type `IFormFile`, but the `ImageFile` property in `Product` is of type `string`.
        // To fix this, we need to handle the conversion properly. For example, we can extract the file name from the `IFormFile` and assign it to the `ImageFile` property in `Product`.

        [Fact]
        public void Insert_ShouldInsertProductAndReturnId()
        {
            // Arrange
            var productModel = new ProductModel
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 19.99m,
                Stock = 50,
                ImageUrl = "http://example.com/image.jpg",
                ImageFile = MockImageFile(), // Use the mock IFormFile method
                ImagePath = "/images/products/image.jpg",
                CreateDate = DateTime.UtcNow,
                IsFeatured = true,
                IsOutOfStock = false,
                Items = "Item1,Item2"
            };

            var productEntity = new Product
            {
                Id = 1,
                Name = productModel.Name,
                Description = productModel.Description,
                Price = productModel.Price,
                Stock = productModel.Stock,
                ImageUrl = productModel.ImageUrl,
                ImageFile = productModel.ImageFile.FileName, // Extract the file name from IFormFile
                ImagePath = productModel.ImagePath,
                CreateDate = productModel.CreateDate,
                IsFeatured = productModel.IsFeatured,
                IsOutOfStock = productModel.IsOutOfStock,
                Items = productModel.Items
            };

            _mapperMock.Setup(m => m.Map<Product>(productModel)).Returns(productEntity);
            _unitOfWorkMock.Setup(u => u.ProductRepository.Insert(It.IsAny<Product>()));
            _unitOfWorkMock.Setup(u => u.SaveChanges());

            // Act
            var resultId = _service.Insert(productModel);

            // Assert
            Assert.Equal(1, resultId);
            _unitOfWorkMock.Verify(u => u.ProductRepository.Insert(productEntity), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }
        [Fact]
        public void Delete_ShouldDeleteProduct_WhenExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Product",
                Description = "Desc",
                Price = 10.5m,
                Stock = 5,
                ImageUrl = "url",
                ImageFile = "file",
                ImagePath = "path",
                CreateDate = DateTime.UtcNow,
                IsFeatured = false,
                IsOutOfStock = false,
                Items = "Item1"
            };
            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>())).Returns(new List<Product> { product }.AsQueryable());

            // Act
            _service.Delete(1);

            // Assert
            _unitOfWorkMock.Verify(u => u.ProductRepository.Delete(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }
        [Fact]
        public void Update_ShouldUpdateProduct_WhenExists()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Desc",
                Price = 9.99m,
                Stock = 10,
                ImageUrl = "oldurl",
                ImageFile = "oldfile",
                ImagePath = "oldpath",
                CreateDate = DateTime.UtcNow,
                IsFeatured = false,
                IsOutOfStock = false,
                Items = "OldItem"
            };

            var updatedProduct = new Product
            {
                Id = 1,
                Name = "New Name",
                Description = "New Desc",
                Price = 19.99m,
                Stock = 20,
                ImageUrl = "newurl",
                ImageFile = "newfile",
                ImagePath = "newpath",
                CreateDate = DateTime.UtcNow,
                IsFeatured = true,
                IsOutOfStock = false,
                Items = "NewItem"
            };

            var productModel = new ProductModel
            {
                Id = 1, // Fix for CS9035: Required member 'ProductModel.Id' must be set
                Name = "Test Product",
                Description = "Test Description",
                Price = 19.99m,
                Stock = 50,
                ImageUrl = "http://example.com/image.jpg",
                ImageFile = MockImageFile(), // Use the mock IFormFile method
                ImagePath = "/images/products/image.jpg",
                CreateDate = DateTime.UtcNow,
                IsFeatured = true,
                IsOutOfStock = false,
                Items = "Item1,Item2"
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(new List<Product> { existingProduct }.AsQueryable());

            _mapperMock.Setup(m => m.Map<Product>(productModel)).Returns(updatedProduct);

            // Act
            _service.Update(1, productModel);

            // Assert
            _unitOfWorkMock.Verify(u => u.ProductRepository.Update(updatedProduct), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateStock_ShouldUpdateStock_WhenProductExists()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Desc",
                Price = 10,
                Stock = 10,
                ImageUrl = "url",
                ImageFile = "file",
                ImagePath = "path",
                CreateDate = DateTime.UtcNow,
                IsFeatured = false,
                IsOutOfStock = false,
                Items = "Item"
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(new List<Product> { product }.AsQueryable());

            _service.UpdateStock(1, 50);

            Assert.Equal(50, product.Stock);
            _unitOfWorkMock.Verify(u => u.ProductRepository.Update(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddReview_ShouldInsertReview_WhenProductExists()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product",
                Description = "Desc", // Fix for CS9035: Required member 'Product.Description' must be set  
                Price = 10.5m,        // Fix for CS9035: Required member 'Product.Price' must be set  
                Stock = 5,            // Fix for CS9035: Required member 'Product.Stock' must be set  
                ImageUrl = "url",     // Fix for CS9035: Required member 'Product.ImageUrl' must be set  
                ImageFile = "file",
                ImagePath = "path",
                CreateDate = DateTime.UtcNow,
                IsFeatured = false,
                IsOutOfStock = false,
                Items = "Item1"
            };

            var reviewModel = new ReviewModel
            {
                Comment = "Great product!"
            };

            var reviewEntity = new Review
            {
                Comment = "Great product!"
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(new List<Product> { product }.AsQueryable());

            _mapperMock.Setup(m => m.Map<Review>(reviewModel)).Returns(reviewEntity);

            _service.AddReview(1, reviewModel);

            _unitOfWorkMock.Verify(u => u.ReviewRepository.Insert(It.IsAny<Review>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ToggleAvailability_ShouldToggleStatus_WhenProductExists()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product",
                IsOutOfStock = false,
                Description = "Desc",
                Price = 10,
                Stock = 10,
                ImageUrl = "url",
                ImageFile = "file",
                ImagePath = "path",
                CreateDate = DateTime.UtcNow,
                IsFeatured = false,
                Items = "Item"
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(new List<Product> { product }.AsQueryable());

            _service.ToggleAvailability(1);

            Assert.True(product.IsOutOfStock);
            _unitOfWorkMock.Verify(u => u.ProductRepository.Update(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ToggleFeatured_ShouldToggleStatus_WhenProductExists()
        {
            var product = new Product
            {
                Id = 1,
                Name = "Product",
                IsFeatured = false,
                Description = "Desc",
                Price = 10,
                Stock = 10,
                ImageUrl = "url",
                ImageFile = "file",
                ImagePath = "path",
                CreateDate = DateTime.UtcNow,
                IsOutOfStock = false,
                Items = "Item"
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(new List<Product> { product }.AsQueryable());

            _service.ToggleFeatured(1);

            Assert.True(product.IsFeatured);
            _unitOfWorkMock.Verify(u => u.ProductRepository.Update(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetLowStockProducts_ShouldReturnMappedProducts()
        {
            var products = new List<Product>
    {
        new Product
        {
            Id = 1,
            Name = "Product",
            Description = "Desc",
            Price = 10,
            Stock = 2,
            ImageUrl = "url",
            ImageFile = "file",
            ImagePath = "path",
            CreateDate = DateTime.UtcNow,
            IsFeatured = false,
            IsOutOfStock = false,
            Items = "Item"
        }
    };

            var mapped = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1,
                    Name = "Sample Product", // Fix for CS9035: Required member 'ProductModel.Name' must be set
                    Description = "Sample Description", // Fix for CS9035: Required member 'ProductModel.Description' must be set
                    Price = 10.99m, // Fix for CS9035: Required member 'ProductModel.Price' must be set
                    Stock = 5, // Fix for CS9035: Required member 'ProductModel.Stock' must be set
                    ImagePath = "/images/sample.jpg", // Fix for CS9035: Required member 'ProductModel.ImagePath' must be set
                    IsFeatured = false, // Fix for CS9035: Required member 'ProductModel.IsFeatured' must be set
                    ImageFile = MockImageFile() // Fix for CS9035: Required member 'ProductModel.ImageFile' must be set
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                           .Returns(products.AsQueryable());

            _mapperMock.Setup(m => m.Map<List<ProductModel>>(products)).Returns(mapped);

            var result = _service.GetLowStockProducts();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        private IFormFile MockImageFile()
        {
            var content = "Fake image content";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            return new FormFile(ms, 0, ms.Length, "ImageFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}