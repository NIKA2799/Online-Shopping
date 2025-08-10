using Dto;
using Interface.Model;
using Microsoft.AspNetCore.Http;

namespace Webdemo.Models
{
    public class ProductModel:  IEntityModel
    {
        public required string ImageUrl;

        public  int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public  string ImagePath { get; set; }
        public  bool IsFeatured { get; set; }
        public bool IsOutOfStock { get; set; }
        public DateTime CreateDate { get; set; }
        public  IFormFile ImageFile { get; set; }
        public  string Items { get; set; }
    }
}
