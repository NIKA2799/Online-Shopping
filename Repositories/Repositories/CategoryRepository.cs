using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using Interface.IRepositories;
namespace Repositories.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(WebDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
