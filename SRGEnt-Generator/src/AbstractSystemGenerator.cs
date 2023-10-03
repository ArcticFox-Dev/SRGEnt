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
    public abstract class {domain.ExecuteSystemName} : ExecuteSystem<{domain.Entity.EntityTypeName},{domain.DomainName},{domain.Entity.EntityMatcherName}, {domain.Entity.EntityAspectSetterName}>
    {{
        protected {domain.ExecuteSystemName}({domain.DomainName} domain, bool shouldSort = false) : base(domain, shouldSort)
        {{
        }}
    }}
}}
";
            var reactiveSystem =  $@"{GeneratorConstants.GeneratorHeader}
using SRGEnt.Systems;

namespace SRGEnt.Generated
{{
    public abstract class {domain.ReactiveSystemName} : ReactiveSystem<{domain.Entity.EntityTypeName},{domain.DomainName},{domain.Entity.EntityMatcherName}, {domain.Entity.EntityAspectSetterName}>
    {{
        protected {domain.ReactiveSystemName}({domain.DomainName} domain, bool shouldSort = false) : base(domain, shouldSort)
        {{
        }}
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,executeSystem,domain.ExecuteSystemName);
            FormattedFileWriter.WriteSourceFile(context,reactiveSystem,domain.ReactiveSystemName);
        }
    }
}