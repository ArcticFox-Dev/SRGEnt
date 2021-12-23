using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.DataTypes
{
    public class Component
    {
        public string Name => _symbol.Name;
        public ITypeSymbol Type => _symbol.Type;
        public bool IsFlag => _symbol.Type.Name.Equals("bool") || _symbol.Type.Name.Equals("Boolean");
        public string InterfaceName => IsFlag ? $"I{_symbol.Name}Flag" : $"I{_symbol.Name}<{_symbol.Type}>";
        public string InterfaceFileName => IsFlag ? $"I{_symbol.Name}Flag" : $"I{_symbol.Name}";
        public string IndexInterfaceName => $"{InterfaceFileName}{IndexType}Index";

        public string AspectName => $"{_symbol.Name.ToUpper()}_INDEX";

        public readonly bool IsObservable;
        public readonly string ObserverScope;
        public readonly bool IsIndex;
        public readonly string IndexType;
        
        private readonly IPropertySymbol _symbol;

        public Component(IPropertySymbol symbol)
        {
            _symbol = symbol;
            var attributeData = symbol.GetAttributes();
            foreach (var attribute in attributeData)
            {
                if (AttributeUtils.IsObservableComponentAttribute(attribute))
                {
                    IsObservable = true;
                    var argument = DatatypeUtils.GetObservableComponentArgumentValue(attribute);
                    ObserverScope = string.IsNullOrEmpty(argument) ? "Entity" : argument;
                }
                else if(AttributeUtils.IsIndexComponentAttribute(attribute))
                {
                    IsIndex = true;
                    var argument = DatatypeUtils.GetIndexComponentArgumentValue(attribute);
                    IndexType = string.IsNullOrEmpty(argument) ? "Primary" : argument;
                }
            }
        }
    }
}