using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    /// <summary>
    /// Represents an entity with a primary key.
    /// </summary>
    public interface IEntity<out T>
    {
        T Id { get; }
    }

    /// <summary>
    /// Represents an entity with an int primary key.
    /// </summary>
    public interface IEntity : IEntity<int>
    {
    }
}
