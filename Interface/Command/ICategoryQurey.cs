using Dto;
using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface.Queries
{
    public interface ICategoryQurey : IQueryModel<CategoryModel,Category>
    {
        IEnumerable<ProductModel> GetProductsByCategory(int categoryId);
    }
}
