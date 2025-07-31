using Dto;
using Interface.Model;
using Microsoft.AspNetCore.Http;

namespace Webdemo.Models
{
    public class ProductModel:  IEntityModel
    {
        public required string ImageUrl;

        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public required string ImagePath { get; set; }
        public required bool IsFeatured { get; set; }
        public bool IsOutOfStock { get; set; }
        public DateTime CreateDate { get; set; }
        public required IFormFile ImageFile { get; set; }
        public required string Items { get; set; }
    }
}
