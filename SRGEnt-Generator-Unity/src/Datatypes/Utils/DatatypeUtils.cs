using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SRGEnt.Generator.DataTypes.Utils
{
    public static class DatatypeUtils
    {
        public static List<Component> ExtractComponents(ITypeSymbol symbol)
        {
            var propertySymbols = symbol.GetMembers().OfType<IPropertySymbol>();
            return propertySymbols.Select(property => new Component(property)).ToList();
        }

        public static string GetDomainDefinitionArgumentValue(AttributeData attribute)
        {
            var argument = attribute.ApplicationSyntaxReference?.GetSyntax().DescendantNodes()
                .OfType<AttributeArgumentSyntax>().FirstOrDefault();
            return argument is null
                ? string.Empty
                : argument.DescendantNodes().OfType<IdentifierNameSyntax>().First().ToString(); 
        }
        
        public static string GetObservableComponentArgumentValue(AttributeData attribute)
        {
            var argument = attribute.ApplicationSyntaxReference?.GetSyntax().DescendantNodes()
                .OfType<AttributeArgumentSyntax>().FirstOrDefault();
            return argument is null
                ? string.Empty
                : argument.DescendantNodes().OfType<IdentifierNameSyntax>().Last().ToString();
        }
        
        public static string GetIndexComponentArgumentValue(AttributeData attribute)
        {
            var argument = attribute.ApplicationSyntaxReference?.GetSyntax().DescendantNodes()
                .OfType<AttributeArgumentSyntax>().FirstOrDefault();
            return argument is null
                ? string.Empty
                : argument.DescendantNodes().OfType<IdentifierNameSyntax>().Last().ToString();
        } 
    }
}