using SRGEnt.Generator.DataTypes;
using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator
{
    public static class DomainGenerator
    {
        public static void GenerateDomain(GeneratorExecutionContext context, Domain domain)
        {
            var domainSymbolName = domain.DomainName;
            var entityName = domain.Entity.EntityTypeName;

            var domainBlock = $@"{GeneratorConstants.GeneratorHeader}
using System;
using System.Collections.Generic;
using SRGEnt.Interfaces;
using SRGEnt.Enums;
using SRGEnt.Groups;
using SRGEnt.Aspects;
using UnityEngine;

namespace SRGEnt.Generated
{{

    public class {domainSymbolName}Container : ScriptableObject
    {{
        public {domainSymbolName} {domainSymbolName};
    }}

    public partial class {domainSymbolName}
    {{
        public Dictionary<long,{entityName}> EntitiesByUid;

        private partial void ConstructorExtensionLateHook()
        {{
            EntitiesByUid = new Dictionary<long, {entityName}>();

            var container = ScriptableObject.CreateInstance<{domainSymbolName}Container>();
            container.{domainSymbolName} = this;
        }}

        private partial void CreateEntityExtensionLateHook({entityName} entity)
        {{
            EntitiesByUid.Add(entity.UId,entity);
        }}

        private partial void EntityMovedExtensionLateHook(int index)
        {{
            EntitiesByUid[_entities[index].UId] = _entities[index];
        }}

        private partial void EntityRemovedExtensionLateHook({entityName} entity)
        {{
            EntitiesByUid.Remove(entity.UId);
        }}
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,domainBlock,$"{domainSymbolName}_UnityExtension");
        }
    }
}