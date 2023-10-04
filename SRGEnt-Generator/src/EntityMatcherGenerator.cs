using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class EntityMatcherGenerator
    {
        public static void GenerateEntityMatcher(GeneratorExecutionContext context, Domain domain)
        {
            var matcher =  $@"{GeneratorConstants.GeneratorHeader}
//using SRGEnt;
using SRGEnt.Aspects;
using SRGEnt.Interfaces;

namespace SRGEnt.Generated
{{
    public class {domain.EntityMatcherName} : AspectMatcher, IAspectMatcher<{domain.EntityAspectSetterName}>
    {{
        private static readonly Aspect _empty = new Aspect({domain.DomainEntityName}.AspectSize);
        public {domain.EntityMatcherName}() : base({domain.DomainEntityName}.AspectSize)
        {{ }}

        public {domain.EntityAspectSetterName} Requires => new {domain.EntityAspectSetterName}(this, Required);
        public {domain.EntityAspectSetterName} CannotHave => new {domain.EntityAspectSetterName}(this, Forbidden);
        public {domain.EntityAspectSetterName} ShouldHaveAtLeastOneOf => new {domain.EntityAspectSetterName}(this, AnyOf);

        public void RecalculateHash()
        {{
            base.RecalculateHashCode();
        }}
        
        protected override ref readonly Aspect Empty => ref _empty;
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,matcher,domain.EntityMatcherName);
        }
    }    
}

