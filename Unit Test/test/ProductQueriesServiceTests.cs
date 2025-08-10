using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

// მოარგე namespaces შენს 솔უშენზე:
using Interface.IRepositories;
using Interface.Queries;
using Service.QueriesService;
using Webdemo.Models;
using Dto;

public class ProductQueriesServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IProductCategoryRepository> _productCategoryRepo = new();

    private readonly ProductQueriesService _sut;

    public ProductQueriesServiceTests()
    {
        _uow.SetupGet(x => x.ProductRepository).Returns(_productRepo.Object);
        _uow.SetupGet(x => x.ProductCategoryRepository).Returns(_productCategoryRepo.Object);

        //_Generic mapper stubs_
        _mapper
            .Setup(m => m.Map<List<ProductModel>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> src) => src.Select(ToModel).ToList());

        _mapper
            .Setup(m => m.Map<IEnumerable<ProductModel>>(It.IsAny<IEnumerable<Product>>()))
            .Returns((IEnumerable<Product> src) => src.Select(ToModel));

        _mapper
            .Setup(m => m.Map<ProductModel>(It.IsAny<Product>()))
            .Returns((Product p) => ToModel(p));

        _sut = new ProductQueriesService(_uow.Object, _mapper.Object);
    }

    // ---------- FindAll ----------
    [Fact]
    public void FindAll_Should_Map_All_Products()
    {
        var data = TD.L(
            P(1, "A", 10, 5, "img/a.jpg"),
            P(2, "B", 20, 3, "img/b.jpg")
        );

        _productRepo.Setup(r => r.FindAll()).Returns(data.AsQueryable());

        var res = _sut.FindAll().ToList();

        Assert.Equal(2, res.Count);
        Assert.Contains(res, x => x.Name == "A");
        Assert.Contains(res, x => x.Name == "B");
    }

    // ---------- FindByCondition ----------
    [Fact]
    public void FindByCondition_Should_Filter_And_Map()
    {
        var data = TD.L(
            P(1, "Phone X", 900, 5, "img/x.jpg"),
            P(2, "Laptop", 1200, 2, "img/l.jpg")
        );

        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns((Expression<Func<Product, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.FindByCondition(p => p.Name.Contains("Phone")).ToList();

        Assert.Single(res);
        Assert.Equal("Phone X", res[0].Name);
    }

    // ---------- Get ----------
    [Fact]
    public void Get_Should_Map_When_Found()
    {
        var p = P(10, "Item", 50, 2, "img/i.jpg");
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns((Expression<Func<Product, bool>> pred) => new[] { p }.AsQueryable().Where(pred));

        var model = _sut.Get(10);

        Assert.NotNull(model);
        Assert.Equal(10, model.Id);
        Assert.Equal("Item", model.Name);
    }

    [Fact]
    public void Get_Should_Return_Null_When_NotFound()
    {
        _productRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                    .Returns(Enumerable.Empty<Product>().AsQueryable());

        var model = _sut.Get(99);

        Assert.Null(model);
    }

    // ---------- GetProductsByCategory ----------
    [Fact]
    public void GetProductsByCategory_Should_Return_Products_Of_Category()
    {
        var p1 = P(1, "Phone", 100, 5, "img/p.jpg");
        var p2 = P(2, "Tablet", 200, 3, "img/t.jpg");

        var pcs = new List<ProductCategory>
        {
            new ProductCategory { CategoryId = 7, ProductId = 1, Product = p1 },
            new ProductCategory { CategoryId = 7, ProductId = 2, Product = p2 }
        };

        _productCategoryRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<ProductCategory, bool>>>()))
            .Returns((Expression<Func<ProductCategory, bool>> pred) => pcs.AsQueryable().Where(pred));

        var res = _sut.GetProductsByCategory(7).ToList();

        Assert.Equal(2, res.Count);
        Assert.Contains(res, x => x.Name == "Phone");
        Assert.Contains(res, x => x.Name == "Tablet");
    }

    // ---------- SearchProducts ----------
    [Fact]
    public void SearchProducts_Should_Return_Keyword_Matches()
    {
        var data = TD.L(
            P(1, "Gaming Laptop", 1500, 2, "img/1.jpg", desc: "RTX"),
            P(2, "Office Chair", 120, 10, "img/2.jpg", desc: "Comfort"),
            P(3, "Laptop Stand", 30, 15, "img/3.jpg", desc: "Ergo")
        );

        _productRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Expression<Func<Product, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.SearchProducts("Laptop").ToList();

        Assert.Equal(2, res.Count);
        Assert.All(res, x => Assert.Contains("Laptop", x.Name));
    }

    // ---------- GetFeaturedProducts ----------
    [Fact]
    public void GetFeaturedProducts_Should_Return_Only_Featured()
    {
        var data = TD.L(
            P(1, "Featured 1", 100, 1, "img/f1.jpg", featured: true),
            P(2, "Not Featured", 50, 2, "img/nf.jpg", featured: false),
            P(3, "Featured 2", 200, 3, "img/f2.jpg", featured: true)
        );

        _productRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Expression<Func<Product, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.GetFeaturedProducts().ToList();

        Assert.Equal(2, res.Count);
        Assert.All(res, x => Assert.True(x.IsFeatured));
    }

    // ---------- GetProducts (pagination) ----------
    [Fact]
    public void GetProducts_Should_Paginate()
    {
        var data = Enumerable.Range(1, 10)
            .Select(i => P(i, "P" + i, i * 10, stock: i, imageUrl: $"img/{i}.jpg", created: DateTime.UtcNow.AddDays(-i)))
            .ToList();

        _productRepo.Setup(r => r.GetAll()).Returns(data.AsQueryable());

        var res = _sut.GetProducts(pageNumber: 2, pageSize: 3).ToList();

        // Page2 (size3) => items 4,5,6
        Assert.Equal(new[] { 4, 5, 6 }, res.Select(x => x.Id).ToArray());
    }

    // ---------- GetProductsByPriceRange ----------
    [Fact]
    public void GetProductsByPriceRange_Should_Filter_By_MinMax()
    {
        var data = TD.L(
            P(1, "A", 50, 1, "img/1.jpg"),
            P(2, "B", 100, 1, "img/2.jpg"),
            P(3, "C", 150, 1, "img/3.jpg")
        );

        _productRepo
            .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Expression<Func<Product, bool>> pred) => data.AsQueryable().Where(pred));

        var res = _sut.GetProductsByPriceRange(80, 120).ToList();

        Assert.Single(res);
        Assert.Equal(2, res[0].Id);
    }

    // ---------- GetRecentlyAddedProducts ----------
    [Fact]
    public void GetRecentlyAddedProducts_Should_Return_Top10_By_CreateDate_Desc()
    {
        var data = Enumerable.Range(1, 15)
            .Select(i => P(i, "P" + i, 10 + i, 1, $"img/{i}.jpg", created: DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        _productRepo.Setup(r => r.GetAll()).Returns(data.AsQueryable());

        var res = _sut.GetRecentlyAddedProducts().ToList();

        // უნდა იყოს 10 ჩანაწერი, ყველაზე ახალი 1..10
        Assert.Equal(10, res.Count);
        Assert.Equal(Enumerable.Range(1, 10), res.OrderByDescending(x => x.CreateDate).Select(x => x.Id));
    }

    // ---------- GetRelatedProducts ----------
    [Fact]
    public void GetRelatedProducts_Should_Return_Only_Matching_By_Category_Price_Stock_Featured()
    {
        // anchor product (price=100, cats: 1,2)
        var anchor = P(10, "Anchor", 100, 5, "img/a.jpg", created: DateTime.UtcNow, featured: true);
        anchor.ProductCategories = new List<ProductCategory>
        {
            new ProductCategory { CategoryId = 1, ProductId = 10, Product = anchor },
            new ProductCategory { CategoryId = 2, ProductId = 10, Product = anchor },
        };

        // candidate pool
        var good1 = P(11, "G1", 90, 5, "img/g1.jpg", featured: true);   // in range, cat match, stock>0, featured
        good1.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1, ProductId = 11, Product = good1 } };

        var good2 = P(12, "G2", 119, 3, "img/g2.jpg", featured: true);
        good2.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 2, ProductId = 12, Product = good2 } };

        var good3 = P(13, "G3", 80, 1, "img/g3.jpg", featured: true);   // boundary 0.8*100
        good3.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1, ProductId = 13, Product = good3 } };

        var badPrice = P(14, "BadPrice", 70, 5, "img/bp.jpg", featured: true);
        badPrice.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1, ProductId = 14, Product = badPrice } };

        var badCat = P(15, "BadCat", 100, 5, "img/bc.jpg", featured: true);
        badCat.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 99, ProductId = 15, Product = badCat } };

        var badStock = P(16, "BadStock", 100, 0, "img/bs.jpg", featured: true);
        badStock.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1, ProductId = 16, Product = badStock } };

        var badFeatured = P(17, "BadFeat", 100, 5, "img/bf.jpg", featured: false);
        badFeatured.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = 1, ProductId = 17, Product = badFeatured } };

        var all = new List<Product> { anchor, good1, good2, good3, badPrice, badCat, badStock, badFeatured };

        // 1) anchor lookup (FindByCondition + Include + ThenInclude + SingleOrDefault)
        _productRepo
    .Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
    .Returns((Expression<Func<Product, bool>> pred) => all.AsQueryable().Where(pred));



        var result = _sut.GetRelatedProducts(10, take: 4).ToList();

        // გამორიცხულია anchor
        Assert.DoesNotContain(result, x => x.Id == 10);
        // ყველა აკმაყოფილებს პირობებს
        Assert.All(result, x => Assert.True(x.Price >= 80 && x.Price <= 120));
        Assert.All(result, x => Assert.True(x.Stock > 0));
        Assert.All(result, x => Assert.True(x.IsFeatured));
        Assert.InRange(result.Count, 1, 4); // მაქს 4, შეიძლება 3ეც იყოს (good1,good2,good3)
    }

    // ----------------- helpers -----------------
    private static ProductModel ToModel(Product p) => new ProductModel
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        ImageUrl = p.ImageUrl,
        IsFeatured = p.IsFeatured,
        CreateDate = p.CreateDate
    };

    private static Product P(
        int id, string name, decimal price, int stock, string imageUrl,
        string desc = "Desc", DateTime? created = null, bool featured = false)
        => new Product
        {
            Id = id,
            Name = name,
            Description = desc,
            Price = price,
            Stock = stock,
            ImageUrl = imageUrl,
            CreateDate = created ?? DateTime.UtcNow.AddDays(-id),
            IsFeatured = featured
        };

    private static class TD
    {
        public static List<T> L<T>(params T[] items) => new(items);
    }
}

