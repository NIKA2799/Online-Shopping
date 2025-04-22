using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Model
{
    public interface IEntityModel<out T>
    {
        T Id { get; }
    }

    public interface IEntityModel : IEntityModel<int>
    {

    }
}