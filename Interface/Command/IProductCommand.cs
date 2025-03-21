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
<<<<<<< HEAD
<<<<<<< HEAD
=======

>>>>>>> e723046664ec4a6c84c53737702f92f3d1e48e3c
=======

>>>>>>> e60dc5767613c26c32003ac8da7260ff4814cbbb
    }
}
