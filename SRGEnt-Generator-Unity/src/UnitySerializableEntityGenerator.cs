using System.Text;
using SRGEnt.Generator.DataTypes;
using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator.Unity
{
    public class UnitySerializableEntityGenerator
    {
        public static void GenerateUnitySerializableEntity(GeneratorExecutionContext context, Domain domain)
        {
            var entityName = domain.Entity.EntityTypeName;

            var propertyBlockBuilder = new StringBuilder();
            var populateBlockBuilder = new StringBuilder();

            for (var i = 0; i < domain.Entity.Components.Count; i++)
            {
                var component = domain.Entity.Components[i];
                propertyBlockBuilder.AppendLine(GenerateEntityPropertyBlock(component));
                populateBlockBuilder.AppendLine(GenerateEntityPopulateBlock(component));
            }

            var entitySource = $@"{GeneratorConstants.GeneratorHeader}
#if UNITY_EDITOR
using System;
using SRGEnt.Interfaces;
using SRGEnt.Aspects;
using SRGEnt.Enums;
using UnityEngine;

namespace SRGEnt.Generated
{{
    [Serializable]
    public class Serializable{entityName} : ScriptableObject
    {{
        public long UId;
{propertyBlockBuilder}

        public void Populate({domain.Entity.EntityTypeName} entity)
        {{
            UId = entity.UId;
{populateBlockBuilder}
        }}
    }}
}}
#endif";
            FormattedFileWriter.WriteSourceFile(context, entitySource, $"Serializable{entityName}");
        }

        private static string GenerateEntityPropertyBlock(Component component)
        {
            var type = component.Type;
            var name = component.Name;
            if (component.IsFlag)
            {
                return $@"        public {type} {name};";
            }
            else
            {
                return $@"        public {type} {name};
        public bool Has{name};";
            }
        }
        
        private static string GenerateEntityPopulateBlock(Component component)
        {
            var name = component.Name;
            if (component.IsFlag)
            {
                return $@"          {name} = entity.{name};";
            }
            else
            {
                return $@"          {name} = entity.Has{name} ? entity.{name} : default;
            Has{name} = entity.Has{name};";
            }
        }

    }
}