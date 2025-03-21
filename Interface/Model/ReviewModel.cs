
using Dto;

namespace Webdemo.Models
{

    public class ReviewModel : IEntityModel

    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int CustomerId { get; set; }

        public int Rating { get; set; } // e.g., a rating from 1 to 5

        public string Comment { get; set; }

        public DateTime DatePosted { get; set; }
    }
}
