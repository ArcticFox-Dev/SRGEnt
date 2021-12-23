using System;

namespace SRGEnt.Interfaces
{
    public interface IDomainObserverToken : IDisposable
    {
        bool Enable { get; set; }
    }
}