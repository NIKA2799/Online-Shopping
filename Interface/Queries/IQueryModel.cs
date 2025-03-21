using Dto;
using System.Linq.Expressions;

namespace Interface.Queries
{
    public interface IQueryModel<TQuery, TResponse>
    where TQuery : IEntityModel
    where TResponse : class, IEntity

    {
        TQuery Get(int id);
        IEnumerable<TQuery> FindByCondition(Expression<Func<TResponse, bool>> predicate);
        IEnumerable<TQuery> FindAll();

    }
}
