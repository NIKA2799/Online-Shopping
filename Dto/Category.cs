using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class Category: IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}
