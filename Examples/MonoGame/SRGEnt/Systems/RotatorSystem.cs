using System;
using SRGEnt.Generated;

namespace ECS_Mono_Example;

/// <summary>
/// This is a rotator system which is of the ExecuteSystem type.
/// SpriteBatchExecuteSystem class is auto generated and saves the
/// user the hassle of declaring generic types for the ExecuteSystem class.
///
/// The system iterates over all entities that have the Angle component
/// and increases the angle by the rotation speed provided via the constructor.
/// 
/// Rotation speed is in degrees per second. 
/// </summary>
public class RotatorSystem : SpriteBatchExecuteSystem
{
    private readonly float _rotationSpeed;

    /// <summary>
    /// The constructor of the rotator system
    /// </summary>
    /// <param name="domain">The domain from which the entities will be queried</param>
    /// <param name="rotationSpeed">Rotation speed of the entities in Degrees per Second</param>
    /// <param name="shouldSort">A flag controlling if the entity span should be sorted by EntityID
    /// (can provide better performance in some cases thanks to lowering the number of cache misses)</param>
    public RotatorSystem(SpriteBatchDomain domain, float rotationSpeed, bool shouldSort = false) : base(domain, shouldSort)
    {
        _rotationSpeed = rotationSpeed;
    }

    protected override void SetMatcher(ref SpriteBatchMatcher matcher)
    {
        matcher.Requires
            .Angle();
    }

    protected override void Execute(ReadOnlySpan<SpriteBatchEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.Angle = (entity.Angle + _domain.ElapsedTime * _rotationSpeed) % 360;
        }
    }
}