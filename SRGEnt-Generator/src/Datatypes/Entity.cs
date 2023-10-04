using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.DataTypes
{
    public class Entity
    {
        private string EntityTypeName { get; }
        public List<Component> Components { get; }

        private readonly ITypeSymbol _symbol;

        public Entity(ITypeSymbol symbol)
        {
            _symbol = symbol;
            EntityTypeName = symbol.Name.Substring(1);

            Components = DatatypeUtils.ExtractComponents(symbol);
        }
    }
}