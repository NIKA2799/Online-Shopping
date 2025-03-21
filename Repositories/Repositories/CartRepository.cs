using Dto;
using Interface.IRepositories;

namespace Repositories.Repositories
{
    public class CartRepository : RepositoryBase<Cart>, ICartRepository
    {
        public CartRepository(WebDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
