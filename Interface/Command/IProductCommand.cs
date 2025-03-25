using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface.Command
{
    public interface IProductCommand: ICommandModel<ProductModel>
    {
        public void UpdateStock(int productId, int newStock);

    }
}
