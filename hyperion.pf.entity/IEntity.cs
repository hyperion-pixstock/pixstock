using System;

namespace Hyperion.Pf.Entity
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
