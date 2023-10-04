using System;

namespace SRGEnt.Interfaces
{
    public interface IEntityObserverToken : IDisposable
    {
        bool Enabled { get; set;}
    }
}