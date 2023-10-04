using System;
using SRGEnt.Generator.DataTypes.Utils;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class ComponentIndexBlockGenerator
    {
        private const string IndexErrorMessage = "Unhandled IndexType";
        private const string DuplicatePrimaryKeyErrorMessage = "Trying to set duplicate PrimaryIndex key";
        
        public static string GenerateComponentIndexContainer(Component component, Domain domain)
        {
            switch(component.IndexType)
            {
                case "Primary" :
                    return
                        $@"        private Dictionary<{component.Type},{domain.DomainEntityName}> _{component.Name.ToCamelCase()}Index = new();";
                case "Secondary":
                    return
                        $@"        private Dictionary<{component.Type},HashSet<{domain.DomainEntityName}>> _{component.Name.ToCamelCase()}Index = new();";
                default: throw new ArgumentException(IndexErrorMessage);
            }
        }

        public static string GenerateComponentIndexGetter(Component component, Domain domain)
        {
            switch(component.IndexType)
            {
                case "Primary" :
                    return GetPrimaryIndexGetter(component, domain);
                case "Secondary":
                    return GetSecondaryIndexGetter(component, domain);
                default: throw new ArgumentException(IndexErrorMessage);
            }
        }

        public static string GenerateComponentIndexUpdaters(Component component, Domain domain)
        {
            switch(component.IndexType)
            {
                case "Primary":
                    return GetPrimaryIndexBlock(component, domain);
                case "Secondary":
                    return GetSecondaryIndexBlock(component, domain);
                default: throw new ArgumentException(IndexErrorMessage);
            };
        }

        private static string GetPrimaryIndexGetter(Component component, Domain domain)
        {
            return $@"        public {domain.DomainEntityName}? GetEntityWith{component.Name}({component.Type} value)
        {{
            if(!_{component.Name.ToCamelCase()}Index.ContainsKey(value)) return null;
            return _{component.Name.ToCamelCase()}Index[value]; 
        }}";
        }

        private static string GetSecondaryIndexGetter(Component component, Domain domain)
        {
            return $@"        public ReadOnlySpan<{domain.DomainEntityName}> GetEntitiesWith{component.Name}({component.Type} value)
        {{
            if(!_{component.Name.ToCamelCase()}Index.ContainsKey(value)) return ReadOnlySpan<{domain.DomainEntityName}>.Empty;
            var entities = new {domain.DomainEntityName}[_{component.Name.ToCamelCase()}Index[value].Count];
            _{component.Name.ToCamelCase()}Index[value].CopyTo(entities);
            return new (entities);
        }}";
        }

        private static string GetPrimaryIndexBlock(Component component, Domain domain)
        {
            var CheckForComponentPresence = component.IsFlag ? $"{component.Name}" : $"Has{component.Name}";
            return $@"        private void IndexBy{component.Name}({domain.DomainEntityName} entity)
        {{
            if (_{component.Name.ToCamelCase()}Index.ContainsKey(entity.{component.Name}))
            {{
                throw new ArgumentException(""{DuplicatePrimaryKeyErrorMessage}"");
            }}

            _{component.Name.ToCamelCase()}Index.Add(entity.{component.Name},entity);
        }}
        private void RemoveFrom{component.Name}Index({domain.DomainEntityName} entity)
        {{
            if(!entity.{CheckForComponentPresence}) return;
            _{component.Name.ToCamelCase()}Index.Remove(entity.{component.Name});
        }}";
        }

        private static string GetSecondaryIndexBlock(Component component, Domain domain)
        {
            var CheckForComponentPresence = component.IsFlag ? $"{component.Name}" : $"Has{component.Name}";
            return $@"        private void IndexBy{component.Name}({domain.DomainEntityName} entity)
        {{
            if (!_{component.Name.ToCamelCase()}Index.ContainsKey(entity.{component.Name}))
            {{
                _{component.Name.ToCamelCase()}Index[entity.{component.Name}] = new ();
            }}

            _{component.Name.ToCamelCase()}Index[entity.{component.Name}].Add(entity);
        }}
        private void RemoveFrom{component.Name}Index({domain.DomainEntityName} entity)
        {{
            if(!entity.{CheckForComponentPresence}) return;
            _{component.Name.ToCamelCase()}Index[entity.{component.Name}].Remove(entity);
        }}
";
        }
    }
}
