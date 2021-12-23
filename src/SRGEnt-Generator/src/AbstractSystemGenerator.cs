using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public class AbstractSystemGenerator
    {
        public static void GenerateAbstractSystems(GeneratorExecutionContext context, Domain domain)
        {
            var executeSystem =  $@"{GeneratorConstants.GeneratorHeader}
using SRGEnt.Systems;

namespace SRGEnt.Generated
{{
    public abstract class {domain.Entity.ExecuteSystemName} : ExecuteSystem<{domain.Entity.EntityTypeName},{domain.DomainName},{domain.Entity.EntityMatcherName}, {domain.Entity.EntityAspectSetterName}>
    {{
        protected {domain.Entity.ExecuteSystemName}({domain.DomainName} domain) : base(domain)
        {{
        }}
    }}
}}
";
            var reactiveSystem =  $@"{GeneratorConstants.GeneratorHeader}
using SRGEnt.Systems;

namespace SRGEnt.Generated
{{
    public abstract class {domain.Entity.ReactiveSystemName} : ReactiveSystem<{domain.Entity.EntityTypeName},{domain.DomainName},{domain.Entity.EntityMatcherName}, {domain.Entity.EntityAspectSetterName}>
    {{
        protected {domain.Entity.ReactiveSystemName}({domain.DomainName} domain) : base(domain)
        {{
        }}
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,executeSystem,domain.Entity.ExecuteSystemName);
            FormattedFileWriter.WriteSourceFile(context,reactiveSystem,domain.Entity.ReactiveSystemName);
        }
    }
}