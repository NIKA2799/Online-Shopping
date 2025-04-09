using Dto;
using System.Collections;
public class Product : IEntity

{
    public ICollection<ProductCategory> ProductCategories { get; set; }

    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Stock { get; set; }

    public required string ImageUrl { get; set; }
    public string ImageFile { get; set; }
    public string ImagePath { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsOutOfStock { get; set; }
    public string Items { get; set; }
    
}



