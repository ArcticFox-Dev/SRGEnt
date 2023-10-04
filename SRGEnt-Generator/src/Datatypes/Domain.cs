using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.DataTypes
{
    public class Domain
    {
        public string DomainFullName { get; }
        public string DomainShortName { get; }
        public string DomainEntityName { get; }
        public string ExecuteSystemName { get; }
        public string ReactiveSystemName { get; }
        public string EntityMatcherName { get; }
        public string EntityAspectSetterName { get; }
        public Entity Entity { get; }
        public List<Component> Components { get; }
        
        private readonly ITypeSymbol _symbol;

        public Domain(ITypeSymbol symbol, Entity entity)
        {
            _symbol = symbol;
            DomainFullName = symbol.Name.StartsWith("I") ? symbol.Name.Substring(1) : symbol.Name;
            DomainShortName = DomainFullName.Replace("Domain", "");
            DomainEntityName = $"{DomainShortName}Entity";
            ExecuteSystemName = $"{DomainShortName}ExecuteSystem";
            ReactiveSystemName = $"{DomainShortName}ReactiveSystem";
            EntityMatcherName = $"{DomainShortName}Matcher";
            EntityAspectSetterName = $"{DomainShortName}AspectSetter";
            
            Entity = entity;
            Components = DatatypeUtils.ExtractComponents(symbol);
        }
    }
}