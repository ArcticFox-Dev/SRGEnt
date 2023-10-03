using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator
{
    public class SRGEntDefinitionReceiver : ISyntaxReceiver
    {
        public readonly List<string> EntitiesToGenerate = new List<string>();
        public readonly List<string> DomainsToGenerate = new List<string>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (!(syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax)) return;

            if (IsEntityDefinition(syntaxNode))
            {
                var entityFullName = GetBaseTypeFullName(interfaceDeclarationSyntax);
                EntitiesToGenerate.Add(entityFullName);
            }
            else if (IsDomainDefinition(syntaxNode))
            {
                var domainFullName = GetBaseTypeFullName(interfaceDeclarationSyntax);
                DomainsToGenerate.Add(domainFullName);
            }
        }

        private static bool IsDomainDefinition(SyntaxNode syntaxNode)
        {
            return syntaxNode.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Any(AttributeUtils.IsDomainDefinitionAttribute);
        }

        private static bool IsEntityDefinition(SyntaxNode syntaxNode)
        {
            return syntaxNode.DescendantNodes()
                .OfType<AttributeSyntax>()
                .Any(AttributeUtils.IsEntityDefinitionAttribute);
        }

        private static string GetBaseTypeFullName(BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
        {
            if (baseTypeDeclarationSyntax.Parent is null) return baseTypeDeclarationSyntax.Identifier.Text;
            return $"{GetNodeFullName(baseTypeDeclarationSyntax.Parent)}.{baseTypeDeclarationSyntax.Identifier.Text}";
        }

        private static string GetNodeFullName(SyntaxNode node)
        {
            var name = "";
            if (node.Parent != null)
            {
                var parentName = GetNodeFullName(node.Parent);
                name = string.IsNullOrEmpty(parentName) ? name : name + ".";
            }
            if (node is ClassDeclarationSyntax classNode)
            {
                name += classNode.Identifier.Text;
            }

            if (node is NamespaceDeclarationSyntax namespaceNode)
            {
                name += namespaceNode.Name;
            }

            return name;
        }
    }
}