using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;
using SRGEnt.Generator.DataTypes.Utils;

namespace SRGEnt.Generator.Unity
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
                    DomainGenerator.GenerateDomain(context, domain);
                    DomainInspectorGenerator.GenerateDomainInspector(context,domain);
                    UnitySerializableEntityGenerator.GenerateUnitySerializableEntity(context,domain);
                    UnityInspectorVisualElementGenerator.GenerateUnityInspectorVisualElement(context,domain);
                }
            }
            finally
            {
                if (domainNames.Count > 0)
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
                    assemblyName = assemblyName.Replace("-", "");

                    var generatorStats = $@"
namespace SRGEnt.Generated.{assemblyName}
{{
    public class UnityGeneratorStats
    {{
        public const int DomainCount = {syntaxReceiver.DomainsToGenerate.Count.ToString()};
        public const string assemblyName = ""{assemblyName}""; 
    }}
}}
";
                    FormattedFileWriter.WriteSourceFile(context, generatorStats, $"{assemblyName}.UnityGeneratorStats");
                }
            }

            syntaxReceiver.EntitiesToGenerate.Clear();
        }
    }
}