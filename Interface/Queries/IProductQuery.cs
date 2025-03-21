using Dto;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Webdemo.Models;

namespace Interface.Queries
{
    public interface IProductQuery : IQueryModel<ProductModel, Product>
    {
        public IEnumerable<ProductModel> GetProductsByCategory(int categoryId);
        public IEnumerable<ProductModel> SearchProducts(string keyword);
        public IEnumerable<ProductModel> GetFeaturedProducts();
        public IEnumerable<ProductModel> GetProducts(int pageNumber, int pageSize);
        public IEnumerable<ProductModel> GetProductsByPriceRange(decimal minPrice, decimal maxPrice);
        public IEnumerable<ProductModel> GetRecentlyAddedProducts();

    }
}
