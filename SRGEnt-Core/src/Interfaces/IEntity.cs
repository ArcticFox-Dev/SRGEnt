using SRGEnt.Aspects;

namespace SRGEnt.Interfaces
{
    public interface IEntity
    {
        long UId { get; }
        int Index { get; }
        bool HasBeenDestroyed { get; }
        Aspect Aspect { get; }
    }
}