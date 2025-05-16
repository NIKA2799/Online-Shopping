using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interface.IRepositories
{
    public interface IOrderDetailRepository : IRepositoryBase<OrderDetail>
    {
        void FindByCondition(Expression<Func<Product, bool>> expression);
    }
}
