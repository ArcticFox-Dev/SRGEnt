using SRGEnt.Attributes;

namespace WobblyAdventures.ECS.Definitions
{
    [EntityDefinition]
    public interface ITestEntity
    {}

    [DomainDefinition(typeof(ITestEntity))]
    public interface ITestDomain
    {}
}