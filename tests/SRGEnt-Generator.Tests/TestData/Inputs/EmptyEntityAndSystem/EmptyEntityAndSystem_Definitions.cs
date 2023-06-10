using SRGEnt.Attributes;

namespace EmptyEntityAndSystem.Definitions
{
    [EntityDefinition]
    public interface ITestEntity
    {}

    [DomainDefinition(typeof(ITestEntity))]
    public interface ITestDomain
    {}
}