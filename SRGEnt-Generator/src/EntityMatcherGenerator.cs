using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class EntityMatcherGenerator
    {
        public static void GenerateEntityMatcher(GeneratorExecutionContext context, Entity entity)
        {
            var matcher =  $@"{GeneratorConstants.GeneratorHeader}
//using SRGEnt;
using SRGEnt.Aspects;
using SRGEnt.Interfaces;

namespace SRGEnt.Generated
{{
    public class {entity.EntityMatcherName} : AspectMatcher, IAspectMatcher<{entity.EntityAspectSetterName}>
    {{
        private static readonly Aspect _empty = new Aspect({entity.EntityTypeName}.AspectSize);
        public {entity.EntityMatcherName}() : base({entity.EntityTypeName}.AspectSize)
        {{ }}

        public {entity.EntityAspectSetterName} Requires => new {entity.EntityAspectSetterName}(this, Required);
        public {entity.EntityAspectSetterName} CannotHave => new {entity.EntityAspectSetterName}(this, Forbidden);
        public {entity.EntityAspectSetterName} ShouldHaveAtLeastOneOf => new {entity.EntityAspectSetterName}(this, AnyOf);

        public void RecalculateHash()
        {{
            base.RecalculateHashCode();
        }}
        
        protected override ref readonly Aspect Empty => ref _empty;
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,matcher,entity.EntityMatcherName);
        }
    }    
}

