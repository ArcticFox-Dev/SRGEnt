using System;

namespace SRGEnt.Interfaces
{
    public interface ICachingEntityGroup<T> : IEntityGroup<T> where T : struct, IEntity, IEquatable<T>
    {
        int EntityCount { get; }
        void UpdateCachedEntities();
        ReadOnlySpan<T> GetCachedEntities(bool update);
        void EntityDestroyed(T entity);
        void EntityAspectChanged(T entity);
        void EntityMoved(T entity);
        void EntityValueChanged(T entity);
    }
}