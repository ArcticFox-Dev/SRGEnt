using SRGEnt.Attributes;

namespace OneComponentEntity.Definitions
{
    [EntityDefinition]
    public interface ISingleComponentTestEntity
    {
        int FirstComponent { get; }
    }

    [DomainDefinition(typeof(ISingleComponentTestEntity))]
    public interface ISingleComponentTestDomain
    {}
}
