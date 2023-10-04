using System;
using System.Linq;
using SRGEnt.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SRGEnt.Generator.DataTypes.Utils
{
    public static class AttributeUtils
    {
        #region Attribute Names

        private const string EntityDefinitionFullName = nameof(EntityDefinitionAttribute);
        private const string DomainDefinitionFullName = nameof(DomainDefinitionAttribute);
        private const string IndexComponentFullName = nameof(IndexComponentAttribute);
        private const string ObservableComponentFullName = nameof(ObservableComponentAttribute);

        private static readonly string EntityDefinitionShortName =
            EntityDefinitionFullName.Replace(nameof(Attribute), "");

        private static readonly string DomainDefinitionShortName =
            DomainDefinitionFullName.Replace(nameof(Attribute), "");

        private static readonly string IndexComponentShortName =
            IndexComponentFullName.Replace(nameof(Attribute), "");

        private static readonly string ObservableComponentShortName =
            ObservableComponentFullName.Replace(nameof(Attribute), "");
        
        #endregion
        
        public static bool IsIndexComponentAttribute(AttributeSyntax attributeSyntax)
        {
            return attributeSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(ins => ins.DescendantTokens()
                    .Any(st => st.Kind() == SyntaxKind.IdentifierToken
                               && (st.ValueText == IndexComponentFullName
                                   || st.ValueText == IndexComponentShortName)));
        }
        
        public static bool IsIndexComponentAttribute(AttributeData attributeData)
        {
            return attributeData.AttributeClass != null && attributeData.AttributeClass.Name == IndexComponentFullName;
        }

        public static bool IsObservableComponentAttribute(AttributeSyntax attributeSyntax)
        {
            return attributeSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(ins => ins.DescendantTokens()
                    .Any(st => st.Kind() == SyntaxKind.IdentifierToken
                               && (st.ValueText == ObservableComponentFullName
                                   || st.ValueText == ObservableComponentShortName)));
        }

        public static bool IsObservableComponentAttribute(AttributeData attributeData)
        {
            return attributeData.AttributeClass != null && attributeData.AttributeClass.Name == ObservableComponentFullName;
        }

        public static bool IsEntityDefinitionAttribute(AttributeSyntax attributeSyntax)
        {
            return attributeSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(ins => ins.DescendantTokens()
                    .Any(st => st.Kind() == SyntaxKind.IdentifierToken
                               && (st.ValueText == EntityDefinitionFullName
                                   || st.ValueText == EntityDefinitionShortName)));
        }

        public static bool IsEntityDefinitionAttribute(AttributeData attributeData)
        {
            return attributeData.AttributeClass != null && attributeData.AttributeClass.Name == EntityDefinitionFullName;
        }

        public static bool IsDomainDefinitionAttribute(AttributeSyntax attributeSyntax)
        {
            return attributeSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Any(ins => ins.DescendantTokens()
                    .Any(st => st.Kind() == SyntaxKind.IdentifierToken
                               && (st.ValueText == DomainDefinitionFullName
                                   || st.ValueText == DomainDefinitionShortName)));
        }
        
        public static bool IsDomainDefinitionAttribute(AttributeData attributeData)
        {
            return attributeData.AttributeClass != null && attributeData.AttributeClass.Name == DomainDefinitionFullName;
        }
    }
}