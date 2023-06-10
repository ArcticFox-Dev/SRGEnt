using SRGEnt.Attributes;
using SRGEnt.Enums;

namespace OneComponentEntity.Definitions
{
    [EntityDefinition]
    public interface IPrimaryIndexComponentTestEntity
    {
        [IndexComponent]
        int FirstComponent { get; }
        [IndexComponent(IndexType.Primary)]
        int SecondComponent { get; }
    }

    [DomainDefinition(typeof(IPrimaryIndexComponentTestEntity))]
    public interface IPrimaryIndexComponentTestDomain
    {}
}