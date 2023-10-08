using System;
using SRGEnt.Generated;

namespace ECS_Mono_Example;

public class RotatorSystem : SpriteBatchExecuteSystem
{
    private readonly float _rotationSpeed;

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