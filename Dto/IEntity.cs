using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public interface IEntity<out T>
    {
        T Id { get; }
    }

    public interface IEntity : IEntity<int>
    {

    }

    public interface IEntityModel<out T>
    {
        T Id { get; }
    }

    public interface IEntityModel : IEntityModel<int>
    {

    }
}
