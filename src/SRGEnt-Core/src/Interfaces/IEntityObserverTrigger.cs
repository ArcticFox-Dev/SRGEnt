using SRGEnt.Enums;

namespace SRGEnt.Interfaces
{
    public interface IEntityObserverTrigger<in TEntity, in TComponent> where TEntity : struct, IEntity
    {
        void Trigger(TEntity entity, TComponent component, ComponentEventType type);
    }
}