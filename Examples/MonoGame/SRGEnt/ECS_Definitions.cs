using Microsoft.Xna.Framework;
using SRGEnt.Attributes;

// Unfortunately for now Entity and Domain definitions need to be in the old style namespace
// Using File scope namespace will prevent the generator from recognizing them as valid source
namespace ECS_Mono_Example
{
    [EntityDefinition]
    public interface ISpriteEntity
    {
        float Angle { get; }
        Point Size { get; }
        Color Color { get; }
    }

    [DomainDefinition(typeof(ISpriteEntity))]
    public interface ISpriteBatchDomain
    {
        int ElapsedTime { get; }
    }
}