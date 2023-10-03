using System.Text;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class EntityGenerator
    {
        public static void GenerateEntity(GeneratorExecutionContext context, Domain domain,
            ComponentInterfacesGenerator componentInterfacesGenerator)
        {
            var entityName = domain.Entity.EntityTypeName;

            var componentIndicesBlockBuilder = new StringBuilder();
            var propertyBlockBuilder = new StringBuilder();
            var observedComponentBlockBuilder = new StringBuilder();

            var interfaceList = new StringBuilder();
            for (var i = 0; i < domain.Entity.Components.Count; i++)
            {
                var component = domain.Entity.Components[i];
                componentInterfacesGenerator.GenerateComponentInterfacesIfNotPresent(ref context, component);
                interfaceList.Append($", {component.InterfaceName}");
                componentIndicesBlockBuilder.AppendLine(
                    GenerateEntityComponentIndicesBlock(i, component));
                propertyBlockBuilder.AppendLine(GenerateEntityPropertyBlock(component));
                if (component.IsObservable && component.ObserverScope == "Entity")
                {
                    observedComponentBlockBuilder.AppendLine(
                        $@"        public {component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}> Observe{component.Name}(Action<{domain.Entity.EntityTypeName},{component.Type},ComponentEventType> handler)
        {{
            return _domain.Observe{component.Name}(this,handler);
        }}");
                }
            }

            var entitySource = $@"{GeneratorConstants.GeneratorHeader}
using System;
using SRGEnt.Interfaces;
using SRGEnt.Aspects;
//using SRGEnt.Enums;

namespace SRGEnt.Generated
{{
    public partial struct {entityName} : IEntity, IEquatable<{entityName}>{interfaceList.ToString()}
    {{
{componentIndicesBlockBuilder.ToString()}
        public const int ComponentCount = {domain.Entity.Components.Count};
        public const int AspectSize = {1 + domain.Entity.Components.Count / 8};

        private readonly {domain.DomainName} _domain;

        public {entityName}({domain.DomainName} domain, int index, long uId)
        {{
            _domain = domain;
            UId = uId;
            Index = index;
        }}

{GenerateIEntityBlock()}
{GenerateEntityEqualityBlock(domain.Entity)}
{propertyBlockBuilder}
{observedComponentBlockBuilder}
    }}
}}";
            FormattedFileWriter.WriteSourceFile(context, entitySource, entityName);
        }

        private static string GenerateEntityComponentIndicesBlock(int index, Component component)
        {
            return $@"public const int {component.AspectName} = {index};";
        }

        private static string GenerateIEntityBlock()
        {
            return @"        public readonly long UId {get;}
        public readonly int Index {get;}
        public readonly bool HasBeenDestroyed => _domain.HasEntityBeenDestroyed(this);
        public readonly Aspect Aspect => _domain.GetEntityAspect(this);
";
        }

        private static string GenerateEntityPropertyBlock(Component component)
        {
            var type = component.Type;
            var name = component.Name;
            var typeName = type.Name;
            if (!component.IsFlag)
            {
                return $@"        public readonly {type} {name}
        {{
            get => _domain.Get{name}(this);
            set
            {{
                var has{name} = Has{name};
                if(has{name} && !{name}.Equals(value))
                {{
                    _domain.Replace{name}(this, value);
                }}
                else if(!has{name})
                {{
                    _domain.Add{name}(this, value);
                }}
            }}
        }}
        public readonly bool Has{name} => _domain.Has{name}(this);
        public readonly void Remove{name}() => _domain.Remove{name}(this);
";
            }
            else
            {
                return $@"        public readonly {type} {name}
        {{
            get => _domain.Has{name}(this);
            set
            {{
                if({name} == value) return;
                _domain.Set{name}(this,value);
            }}
        }}
";
            }
        }

        private static string GenerateEntityEqualityBlock(Entity entity)
        {
            return $@"        public bool Equals({entity.EntityTypeName} other)
        {{
            return UId == other.UId;
        }}

        public override bool Equals(object obj)
        {{
            return obj is {entity.EntityTypeName} other && Equals(other);
        }}

        public override int GetHashCode()
        {{
            return UId.GetHashCode();
        }}";
        }
    }
}