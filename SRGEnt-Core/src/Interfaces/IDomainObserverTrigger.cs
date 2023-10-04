namespace SRGEnt.Interfaces
{
    public interface IDomainObserverTrigger<in TDomain, in TComponent>
    {
        void Trigger(TDomain domain, TComponent component);
    }
}