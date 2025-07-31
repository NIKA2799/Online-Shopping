using Webdemo.Models;

namespace Interface.Command
{
    public interface IProductCommand : ICommandModel<ProductModel>
    {
        public void UpdateStock(int productId, int newStock);
        void ToggleFeatured(int productId);
        void ToggleAvailability(int productId);
        void AddReview(int productId, ReviewModel reviewModel);
       IEnumerable<ProductModel> GetLowStockProducts(int threshold = 5);
    }
}
