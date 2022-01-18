using System;

namespace SRGEnt.Interfaces
{
    public interface IEntityGroup<T> where T : struct, IEntity, IEquatable<T>
    {
        ReadOnlySpan<T> Entities { get; }
    }
}