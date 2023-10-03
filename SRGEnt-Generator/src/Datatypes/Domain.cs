using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.DataTypes
{
    public class Domain
    {
        public string DomainName { get; }
        public string ExecuteSystemName { get; }
        public string ReactiveSystemName { get; }
        public Entity Entity { get; }
        public List<Component> Components { get; }
        
        private readonly ITypeSymbol _symbol;

        public Domain(ITypeSymbol symbol, Entity entity)
        {
            _symbol = symbol;
            DomainName = symbol.Name.Substring(1);
            ExecuteSystemName = $"{DomainName}ExecuteSystem";
            ReactiveSystemName = $"{DomainName}ReactiveSystem";
            Entity = entity;
            Components = DatatypeUtils.ExtractComponents(symbol);
        }
    }
}