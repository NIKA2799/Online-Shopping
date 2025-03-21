using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
   public interface IShippingService
    {
        ShippingModel GetShippingById(int id);
        IEnumerable<ShippingModel> GetAllShippings();
        int CreateShipping(ShippingModel shippingModel);
        bool UpdateShipping(ShippingModel shippingModel);
        bool DeleteShipping(int id);
    }
}
