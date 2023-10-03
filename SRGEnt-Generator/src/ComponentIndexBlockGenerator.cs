using System;
using SRGEnt.Generator.DataTypes.Utils;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public static class ComponentIndexBlockGenerator
    {
        private const string IndexErrorMessage = "Unhandled IndexType";
        private const string DuplicatePrimaryKeyErrorMessage = "Trying to set duplicate PrimaryIndex key";
        
        public static string GenerateComponentIndexContainer(Component component, Entity entity)
        {
            switch(component.IndexType)
            {
                case "Primary" :
                    return
                        $@"        private Dictionary<{component.Type},{entity.EntityTypeName}> _{component.Name.ToCamelCase()}Index = new();";
                case "Secondary":
                    return
                        $@"        private Dictionary<{component.Type},HashSet<{entity.EntityTypeName}>> _{component.Name.ToCamelCase()}Index = new();";
                default: throw new ArgumentException(IndexErrorMessage);
            }
        }

        public static string GenerateComponentIndexGetter(Component component, Entity entity)
        {
            switch(component.IndexType)
            {
                case "Primary" :
                    return GetPrimaryIndexGetter(component, entity);
                case "Secondary":
                    return GetSecondaryIndexGetter(component, entity);
                default: throw new ArgumentException(IndexErrorMessage);
            }
        }

        public static string GenerateComponentIndexUpdaters(Component component, Entity entity)
        {
            switch(component.IndexType)
            {
                case "Primary":
                    return GetPrimaryIndexBlock(component, entity);
                case "Secondary":
                    return GetSecondaryIndexBlock(component, entity);
                default: throw new ArgumentException(IndexErrorMessage);
            };
        }

        private static string GetPrimaryIndexGetter(Component component, Entity entity)
        {
            return $@"        public {entity.EntityTypeName}? GetEntityWith{component.Name}({component.Type} value)
        {{
            if(!_{component.Name.ToCamelCase()}Index.ContainsKey(value)) return null;
            return _{component.Name.ToCamelCase()}Index[value]; 
        }}";
        }

        private static string GetSecondaryIndexGetter(Component component, Entity entity)
        {
            return $@"        public ReadOnlySpan<{entity.EntityTypeName}> GetEntitiesWith{component.Name}({component.Type} value)
        {{
            if(!_{component.Name.ToCamelCase()}Index.ContainsKey(value)) return ReadOnlySpan<{entity.EntityTypeName}>.Empty;
            var entities = new {entity.EntityTypeName}[_{component.Name.ToCamelCase()}Index[value].Count];
            _{component.Name.ToCamelCase()}Index[value].CopyTo(entities);
            return new (entities);
        }}";
        }

        private static string GetPrimaryIndexBlock(Component component, Entity entity)
        {
            var CheckForComponentPresence = component.IsFlag ? $"{component.Name}" : $"Has{component.Name}";
            return $@"        private void IndexBy{component.Name}({entity.EntityTypeName} entity)
        {{
            if (_{component.Name.ToCamelCase()}Index.ContainsKey(entity.{component.Name}))
            {{
                throw new ArgumentException(""{DuplicatePrimaryKeyErrorMessage}"");
            }}

            _{component.Name.ToCamelCase()}Index.Add(entity.{component.Name},entity);
        }}
        private void RemoveFrom{component.Name}Index({entity.EntityTypeName} entity)
        {{
            if(!entity.{CheckForComponentPresence}) return;
            _{component.Name.ToCamelCase()}Index.Remove(entity.{component.Name});
        }}";
        }

        private static string GetSecondaryIndexBlock(Component component, Entity entity)
        {
            var CheckForComponentPresence = component.IsFlag ? $"{component.Name}" : $"Has{component.Name}";
            return $@"        private void IndexBy{component.Name}({entity.EntityTypeName} entity)
        {{
            if (!_{component.Name.ToCamelCase()}Index.ContainsKey(entity.{component.Name}))
            {{
                _{component.Name.ToCamelCase()}Index[entity.{component.Name}] = new ();
            }}

            _{component.Name.ToCamelCase()}Index[entity.{component.Name}].Add(entity);
        }}
        private void RemoveFrom{component.Name}Index({entity.EntityTypeName} entity)
        {{
            if(!entity.{CheckForComponentPresence}) return;
            _{component.Name.ToCamelCase()}Index[entity.{component.Name}].Remove(entity);
        }}
";
        }
    }
}
