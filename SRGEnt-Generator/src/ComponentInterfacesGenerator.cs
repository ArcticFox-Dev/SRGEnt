using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SRGEnt.Generator.DataTypes;

namespace SRGEnt.Generator
{
    public class ComponentInterfacesGenerator
    {
        private readonly HashSet<string> _componentInterfaces = new HashSet<string>();
        public void GenerateIndexInterfaceIfNotPresent(ref GeneratorExecutionContext context, Component component)
        {
            var metadataName = $"SRGEnt.Generated.{component.IndexInterfaceName}";
            if (_componentInterfaces.Contains(metadataName) 
                || SymbolUtilities.DoesSymbolExist(metadataName,context.Compilation)) return;
            _componentInterfaces.Add(metadataName);

            if (component.IndexType == "Primary")
            {
                var entitySubtype = component.IsFlag ? component.InterfaceName : $"{component.InterfaceFileName}<TComponentType>";
                var componentInterfaceBody = $@"{GeneratorConstants.GeneratorHeader}

namespace SRGEnt.Generated
{{
    public interface {component.IndexInterfaceName}<TEntity,TComponentType> where TEntity : struct, {entitySubtype}
    {{
        TEntity? GetEntityWith{component.Name}(TComponentType value);
    }}
}}";
                FormattedFileWriter.WriteSourceFile(context,componentInterfaceBody,component.IndexInterfaceName);
            }
            else
            {
                var entitySubtype = component.IsFlag ? component.InterfaceName : $"{component.InterfaceFileName}<TComponentType>";
                var componentInterfaceBody = $@"{GeneratorConstants.GeneratorHeader}

namespace SRGEnt.Generated
{{
    public interface {component.IndexInterfaceName}<TEntity,TComponentType> where TEntity : struct, {entitySubtype}
    {{
        ReadOnlySpan<TEntity> GetEntitiesWith{component.Name}(TComponentType value);
    }}
}}";
                FormattedFileWriter.WriteSourceFile(context,componentInterfaceBody,component.IndexInterfaceName);
            }
        }

        public void GenerateComponentInterfacesIfNotPresent(ref GeneratorExecutionContext context, Component component)
        {
            var metadataName = $"SRGEnt.Generated.{component.MetadataNameSuffix}";

            if (_componentInterfaces.Contains(component.InterfaceFileName) 
                || SymbolUtilities.DoesSymbolExist(metadataName,context.Compilation)) return;

            if (component.IsFlag)
            {
                var componentInterfaceBody = $@"{GeneratorConstants.GeneratorHeader}
namespace SRGEnt.Generated
{{
    public interface I{component.Name}Flag
    {{
        {component.Type} {component.Name} {{get; set;}}
    }}
}}";
                FormattedFileWriter.WriteSourceFile(context,componentInterfaceBody,component.InterfaceFileName);
            }
            else
            {
                var componentInterfaceBody = $@"{GeneratorConstants.GeneratorHeader}
namespace SRGEnt.Generated
{{
    public interface I{component.Name}<T>
    {{
        T {component.Name} {{get; set;}}
        bool Has{component.Name} {{get;}}
        void Remove{component.Name}();
    }}
}}
";
                FormattedFileWriter.WriteSourceFile(context,componentInterfaceBody,component.InterfaceFileName);
            }

            var componentAspectBody = $@"{GeneratorConstants.GeneratorHeader}
namespace SRGEnt.Generated
{{
    public interface I{component.Name}Aspect<TAspectSetter>
    {{
        TAspectSetter {component.Name}();
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,componentAspectBody,$"I{component.Name}Aspect");
            _componentInterfaces.Add(component.InterfaceFileName);
        }

        public void GenerateEntityComponentObserverToken(GeneratorExecutionContext context, Domain domain,
            Component component)
        {
            var observerTokenName = $"{component.Name}ObserverToken";
            var metadataName = $"SRGEnt.Generated.{observerTokenName}";
            if (_componentInterfaces.Contains(observerTokenName)
            || SymbolUtilities.DoesSymbolExist(metadataName,context.Compilation)) return;

            var observerTokenBody = $@"{GeneratorConstants.GeneratorHeader}
using System;
using SRGEnt.Generated;
using SRGEnt.Enums;
using SRGEnt.Interfaces;

namespace SRGEnt.Generated
{{
    public class {component.Name}ObserverToken<TEntity,TComponentType> : 
        IDisposable,
        IEntityObserverToken,
        IEntityObserverTrigger<TEntity,TComponentType> where TEntity : struct, IEntity
    {{
        private bool _isDisposed; 
        private event Action <TEntity, TComponentType, ComponentEventType> _handler;

        public bool Enabled {{ get; set;}}

        public {component.Name}ObserverToken(Action<TEntity, TComponentType, ComponentEventType> handler)
        {{
            _handler = handler;
            Enabled = true;
        }}

        public void Trigger(TEntity entity, TComponentType component, ComponentEventType eventType)
        {{
            if(!Enabled) return;
            _handler?.Invoke(entity,component,eventType);
        }}

        public void Dispose()
        {{
            Dispose(true);
            GC.SuppressFinalize(this);
        }}

        protected virtual void Dispose(bool disposing)
        {{
            if (_isDisposed) return;
            if (disposing) _handler = null;
            _isDisposed = true;
        }}
    }}
}}";
            FormattedFileWriter.WriteSourceFile(context, observerTokenBody, $"{observerTokenName}");
        }
    }
}