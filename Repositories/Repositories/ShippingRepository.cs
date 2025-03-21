using Dto;
using Interface.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ShippingRepository : RepositoryBase<Shipping>, IShippingRepository
    {
        public ShippingRepository(WebDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
