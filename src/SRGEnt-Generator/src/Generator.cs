using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SRGEntDefinitionReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var componentInterfaceGenerator = new ComponentInterfacesGenerator();
            // the generator infrastructure will create a receiver and populate it
            // we can retrieve the populated instance via the context
            var syntaxReceiver = (SRGEntDefinitionReceiver)context.SyntaxReceiver;
            if (syntaxReceiver is null) return;

            var entitiesNames = new List<string>();
            var domainNames = new List<string>();
            
            try
            {
                var entities = new Dictionary<string, Entity>();

                foreach (var entityFullType in syntaxReceiver.EntitiesToGenerate)
                {
                    var typeSymbol = context.Compilation.GetTypeByMetadataName(entityFullType);
                    if (typeSymbol is null) continue;
                    entities.Add(typeSymbol.Name, new Entity(typeSymbol));
                    entitiesNames.Add(typeSymbol.Name);
                }

                var domains = new Dictionary<string, Domain>();

                foreach (var domainFullType in syntaxReceiver.DomainsToGenerate)
                {
                    var typeSymbol = context.Compilation.GetTypeByMetadataName(domainFullType);
                    if (typeSymbol is null) continue;
                    var attribute = typeSymbol.GetAttributes().First();
                    var entityType = DatatypeUtils.GetDomainDefinitionArgumentValue(attribute);

                    domains.Add(typeSymbol.Name, new Domain(typeSymbol, entities[entityType]));
                    domainNames.Add(typeSymbol.Name);
                }

                foreach (var nameDomainPair in domains)
                {
                    var domain = nameDomainPair.Value;
                    DomainGenerator.GenerateDomain(context, domain, componentInterfaceGenerator);
                    DomainInspectorGenerator.GenerateDomainInspector(context,domain);
                    EntityGenerator.GenerateEntity(context, domain, componentInterfaceGenerator);
                    UnitySerializableEntityGenerator.GenerateUnitySerializableEntity(context,domain);
                    UnityInspectorVisualElementGenerator.GenerateUnityInspectorVisualElement(context,domain);
                    AspectSetterGenerator.GenerateEntityAspectSetter(context, domain.Entity);
                    EntityMatcherGenerator.GenerateEntityMatcher(context, domain.Entity);
                    AbstractSystemGenerator.GenerateAbstractSystems(context,domain);
                }
            }
            finally
            {
                var sb = new StringBuilder();
                foreach (var entity in entitiesNames)
                {
                    sb.AppendLine($"//{entity}");
                }

                foreach (var name in domainNames)
                {
                    sb.AppendLine($"//{name}");
                }

                var assemblyIdentity = context.Compilation.Assembly.Identity;
                var assemblyName = assemblyIdentity.Name;
                assemblyName = assemblyName.Replace(".", "");
                assemblyName = assemblyName.Replace(",", "");

                var generatorStats = $@"
namespace SRGEnt.Generated.{assemblyName}
{{
    public class GeneratorStats
    {{
        public const int EntityCount = {syntaxReceiver.EntitiesToGenerate.Count.ToString()};
        public const int DomainCount = {syntaxReceiver.DomainsToGenerate.Count.ToString()};
        public const string assemblyName = ""{assemblyName}""; 
    }}
}}
";
                FormattedFileWriter.WriteSourceFile(context, generatorStats, $"{assemblyName}.GeneratorStats");
            }

            syntaxReceiver.EntitiesToGenerate.Clear();
        }
    }
}