using Microsoft.Xna.Framework;
using SRGEnt.Attributes;

// Unfortunately for now Entity and Domain definitions need to be in the old style namespace
// Using File scope namespace will prevent the generator from recognizing them as valid source
namespace ECS_Mono_Example
{
    /// <summary>
    /// The below definition is a template that the generator will use to creat Domain specific entities
    /// for domains that use this type as their Entity.
    /// Each of the properties in the interface will be available as components to be added on the output entity.
    /// </summary>
    [EntityDefinition]
    public interface ISpriteEntity
    {
        float Angle { get; }
        Point Size { get; }
        Color Color { get; }
    }

    /// <summary>
    /// The below definition is a Domain declaration.
    /// It will inform the generator that a domain SpriteBatchDomain
    /// should be created with an Entity conforming to the ISpriteEntity API.
    /// Additionally the domain has a Domain Scope "components" called ElapsedTime and CircleRadius which will be
    /// accessible in all systems that operate on this domain. 
    /// </summary>
    [DomainDefinition(typeof(ISpriteEntity))]
    public interface ISpriteBatchDomain
    {
        int ElapsedTime { get; }
        int CircleRadius { get; }
    }
}