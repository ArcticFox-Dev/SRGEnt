using System;
using SRGEnt.Enums;

namespace SRGEnt.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObservableComponentAttribute : Attribute
    {
        private readonly ObserverScope _observerScope;
        public ObservableComponentAttribute(ObserverScope observerScope = ObserverScope.Entity)
        {
            _observerScope = observerScope;
        }
    }
}