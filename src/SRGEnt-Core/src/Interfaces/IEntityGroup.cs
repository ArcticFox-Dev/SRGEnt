using System;

namespace SRGEnt.Interfaces
{
    public interface IEntityGroup<T> where T : struct, IEntity, IEquatable<T>
    {
        ReadOnlySpan<T> Entities { get; }
        void EntityDestroyed(T entity);
        void EntityAspectChanged(T entity);
        void EntityMoved(T entity);
        void EntityValueChanged(T entity);
    }
}