using Dto;
using Interface.IRepositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class CartItemRepository : RepositoryBase<CartItem>, ICartItemRepository
    {
        public CartItemRepository(WebDemoDbContext dbContext) : base(dbContext)
        {
        }


        public void Insert(Cart cart)
        {
            throw new NotImplementedException();
        }

    }
}
