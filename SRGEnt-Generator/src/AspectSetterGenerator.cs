using System.Text;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class AspectSetterGenerator
    {
        public static void GenerateEntityAspectSetter(GeneratorExecutionContext context, Domain domain)
        {
            var entitySymbolName = domain.DomainEntityName;
            var matcherSymbolName = domain.EntityMatcherName;
            var setterSymbolName = domain.EntityAspectSetterName;

            var propertyBuilder = new StringBuilder();
            var interfaceListBuilder = new StringBuilder();

            foreach (var component in domain.Entity.Components)
            {
                interfaceListBuilder.Append($", I{component.Name}Aspect<{setterSymbolName}>");
                propertyBuilder.AppendLine($@"        public {setterSymbolName} {component.Name}()
        {{
            _aspect.Set(new AspectIndex({entitySymbolName}.{component.AspectName}));
            return this;
        }}");
            }

            var setterBody = $@"{GeneratorConstants.GeneratorHeader}
using SRGEnt.Aspects;
using SRGEnt.Interfaces;

namespace SRGEnt.Generated
{{
    public readonly struct {setterSymbolName} : IAspectMatcher<{setterSymbolName}>{interfaceListBuilder}
    {{
        private readonly {matcherSymbolName} _parent;
        private readonly Aspect _aspect;

        public {setterSymbolName}({matcherSymbolName} parent, Aspect aspect)
        {{
            _parent = parent;
            _aspect = aspect;
        }}

        public {setterSymbolName} Requires => _parent.Requires;
        public {setterSymbolName} CannotHave => _parent.CannotHave;
        public {setterSymbolName} ShouldHaveAtLeastOneOf => _parent.ShouldHaveAtLeastOneOf;

        public void RecalculateHash()
        {{
            _parent.RecalculateHash();
        }}

{propertyBuilder}
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,setterBody,setterSymbolName);
        }
    }
}