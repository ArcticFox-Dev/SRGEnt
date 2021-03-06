using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.DataTypes
{
    public class Entity
    {
        public string EntityTypeName { get; }
        public string EntityMatcherName { get; }
        public string EntityAspectSetterName { get; }
        public string ExecuteSystemName { get; }
        public string ReactiveSystemName { get; }
        public List<Component> Components { get; }

        private readonly ITypeSymbol _symbol;

        public Entity(ITypeSymbol symbol)
        {
            _symbol = symbol;
            EntityTypeName = symbol.Name.Substring(1);
            var entityBaseName = EntityTypeName.Replace("Entity", "");
            EntityMatcherName = $"{entityBaseName}Matcher";
            EntityAspectSetterName = $"{entityBaseName}AspectSetter";
            ExecuteSystemName = $"{entityBaseName}ExecuteSystem";
            ReactiveSystemName = $"{entityBaseName}ReactiveSystem";

            Components = DatatypeUtils.ExtractComponents(symbol);
        }
    }
}