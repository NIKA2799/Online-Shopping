using Dto;
using Interface.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class WishlistRepositorty : RepositoryBase<Wishlist>, IWishlistRepositorty
    {
        public WishlistRepositorty(WebDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
