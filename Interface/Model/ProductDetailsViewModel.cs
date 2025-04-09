using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface.Model
{
    public class ProductDetailsViewModel
    {
        public ProductModel Product { get; set; }
        public List<ProductModel> RelatedProducts { get; set; }
    }
}
