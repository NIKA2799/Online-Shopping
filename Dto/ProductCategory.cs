namespace Dto
{
    public class ProductCategory : IEntity
    {

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int Id => throw new NotImplementedException();
    }
}