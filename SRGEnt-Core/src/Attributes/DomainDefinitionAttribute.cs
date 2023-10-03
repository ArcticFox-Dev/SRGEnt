using System;

namespace SRGEnt.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class DomainDefinitionAttribute : Attribute
    {
        public DomainDefinitionAttribute(Type entityType)
        {}
    }
}