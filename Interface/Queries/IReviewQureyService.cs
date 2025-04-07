using Dto;
using System.Linq.Expressions;
using Webdemo.Models;

namespace Interface.Queries
{
    public interface IReviewQureyService
    {
        IEnumerable<ReviewModel> GetReviewsByProduct(int productId);
        ReviewModel GetReviewByUser(int productId, int customerId);
        IEnumerable<ReviewModel> FindByCondition(Expression<Func<Review, bool>> predicate);
    }
}
