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
    public interface ICategoryQuery : IQueryModel<CategoryModel, Category>
    {
    }
}
