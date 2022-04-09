using System.Text;
using SRGEnt.Generator.DataTypes;
using SRGEnt.Generator.DataTypes.Utils;
using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator
{
    public static class DomainGenerator
    {
        public static void GenerateDomain(GeneratorExecutionContext context, Domain domain, ComponentInterfacesGenerator componentInterfacesGenerator)
        {
            var domainSymbolName = domain.DomainName;
            var entityName = domain.Entity.EntityTypeName;
            var matcherName = domain.Entity.EntityMatcherName;
            var aspectSetterName = domain.Entity.EntityAspectSetterName;

            var indexStorage = new StringBuilder();
            var indexGetters = new StringBuilder();
            var indexBlocks = new StringBuilder();
            var observerStorage = new StringBuilder();
            var observerRegistration = new StringBuilder();
            var observerTrigger = new StringBuilder();
            var observerPresenceChecks = new StringBuilder();
            var removeObservers = new StringBuilder();
            var updateObserverEvents = new StringBuilder();
            var removeFromIndexes = new StringBuilder();
            var domainComponents = new StringBuilder();
            var domainComponentInterfaces = new StringBuilder();
            var domainIndexInterfaces = new StringBuilder();
            var componentArrays = new StringBuilder();
            var arraysInitialization = new StringBuilder();
            var componentFunctionBlock = new StringBuilder();
            var componentCapacityDoubling = new StringBuilder();
            var shiftComponentsBlock = new StringBuilder();
            var updateIndexesBlock = new StringBuilder();

            foreach (var domainComponent in domain.Components)
            {
                componentInterfacesGenerator.GenerateComponentInterfacesIfNotPresent(ref context, domainComponent);
                domainComponentInterfaces.Append($", {domainComponent.InterfaceName}");
                domainComponents.AppendLine(GenerateDomainComponentBody(domainComponent));
            }

            foreach (var component in domain.Entity.Components)
            {
                if (component.IsIndex)
                {
                    indexStorage.AppendLine(
                        ComponentIndexBlockGenerator.GenerateComponentIndexContainer(component, domain.Entity));
                    indexGetters.AppendLine(
                        ComponentIndexBlockGenerator.GenerateComponentIndexGetter(component, domain.Entity));
                    indexBlocks.AppendLine(
                        ComponentIndexBlockGenerator.GenerateComponentIndexUpdaters(component, domain.Entity));
                    componentInterfacesGenerator.GenerateIndexInterfaceIfNotPresent(ref context, component);
                    domainIndexInterfaces.Append(
                        $", {component.IndexInterfaceName}<{domain.Entity.EntityTypeName},{component.Type}>");
                    removeFromIndexes.AppendLine($@"                RemoveFrom{component.Name}Index(entity);");
                    updateIndexesBlock.AppendLine($@"if(Has{component.Name}(entity))
{{
    RemoveFrom{component.Name}Index(_entities[index]);
    IndexBy{component.Name}(_entities[index]);
}}
");
                }

                if (component.IsObservable)
                {
                    componentInterfacesGenerator.GenerateEntityComponentObserverToken(context, domain, component);
                    observerRegistration.AppendLine(GenerateEntityComponentObserverRegistrationBody(domain, component));
                    observerStorage.AppendLine(
                        $"        private Dictionary<{domain.Entity.EntityTypeName}, ComponentEventType> _{component.Name.ToCamelCase()}Events = new();");
                    removeObservers.AppendLine(
                        $@"                _{component.Name.ToCamelCase()}Events.Remove(entity);");
                    updateObserverEvents.AppendLine(
                        $@"                    if(_{component.Name.ToCamelCase()}Events.ContainsKey(_entities[index]))
                    {{
                        var old{component.Name}Event = _{component.Name.ToCamelCase()}Events[_entities[index]];
                        _{component.Name.ToCamelCase()}Events.Remove(_entities[index]);
                        _{component.Name.ToCamelCase()}Events.Add(_entities[index],old{component.Name}Event);
                    }}");
                    if (component.ObserverScope == "Domain")
                    {
                        observerPresenceChecks.AppendLine(
                            $@"        private bool Has{component.Name}Observers({entityName} entity)
        {{
            return _{component.Name.ToCamelCase()}Observers.Count > 0;
        }}");
                        observerStorage.AppendLine(
                            $"        private List<WeakReference<{component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>>> _{component.Name.ToCamelCase()}Observers = new();");
                        observerTrigger.AppendLine(
                            $@"            foreach(var observerRef in _{component.Name.ToCamelCase()}Observers)
            {{
                if(observerRef.TryGetTarget(out var observer))
                {{
                    foreach(var componentEvent in _{component.Name.ToCamelCase()}Events)
                    {{
                        var component = Has{component.Name}(componentEvent.Key) ? componentEvent.Key.{component.Name} : default;
                        observer.Trigger(componentEvent.Key, component, componentEvent.Value);
                    }}
                }}
            }}
            _{component.Name.ToCamelCase()}Events.Clear();");
                    }
                    else
                    {
                        observerPresenceChecks.AppendLine(
                            $@"        private bool Has{component.Name}Observers({entityName} entity)
        {{
            return _{component.Name.ToCamelCase()}Observers.ContainsKey(entity) && _{component.Name.ToCamelCase()}Observers[entity].Count > 0;
        }}");
                        removeObservers.AppendLine(
                            $@"                if(_{component.Name.ToCamelCase()}Observers.ContainsKey(entity)) _{component.Name.ToCamelCase()}Observers.Remove(entity);");
                        observerStorage.AppendLine(
                            $"        private Dictionary<{domain.Entity.EntityTypeName},List<WeakReference<{component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>>>> _{component.Name.ToCamelCase()}Observers = new();");
                        observerTrigger.AppendLine(
                            $@"            foreach(var componentEvent in _{component.Name.ToCamelCase()}Events)
            {{
                if(_{component.Name.ToCamelCase()}Observers.TryGetValue(componentEvent.Key, out var observers))
                {{
                    foreach(var observerRef in observers)
                    {{
                        if(observerRef.TryGetTarget(out var observer))
                        {{
                            var component = Has{component.Name}(componentEvent.Key) ? componentEvent.Key.{component.Name} : default;
                            observer.Trigger(componentEvent.Key, component, componentEvent.Value);
                        }}
                    }}
                }}
            }}
            _{component.Name.ToCamelCase()}Events.Clear();");
                    }
                }

                var componentArrayName = $"_{component.Name.ToCamelCase()}Components";
                var componentName = component.Name;
                var componentType = component.Type;
                if (!component.IsFlag)
                {
                    componentArrays.AppendLine($"        private {componentType}[] {componentArrayName};");
                    arraysInitialization.AppendLine(
                        $"            {componentArrayName} = new {componentType}[CurrentCapacity];");
                    shiftComponentsBlock.AppendLine(GenerateValueComponentCapacityDoublingBody(componentName,
                        componentType, componentArrayName, componentCapacityDoubling));
                    componentFunctionBlock.AppendLine(GenerateComponentFunctionBlock(component, componentName,
                        entityName,
                        component.AspectName, componentArrayName, componentType));
                }
                else
                {
                    componentFunctionBlock.AppendLine(GenerateFlagComponentFunctionBlock(component, componentName,
                        entityName,
                        component.AspectName, componentArrayName, componentType));
                }
            }

            var domainBlock = $@"{GeneratorConstants.GeneratorHeader}
using System;
using System.Collections.Generic;
using SRGEnt.Interfaces;
using SRGEnt.Enums;
using SRGEnt.Groups;
using SRGEnt.Aspects;
#if UNITY_EDITOR
using UnityEngine;
#else
using System.Diagnostics;
#endif

namespace SRGEnt.Generated
{{

    #if UNITY_EDITOR
    public class {domainSymbolName}Container : ScriptableObject
    {{
        public {domainSymbolName} {domainSymbolName};
    }}
    #endif

    public partial class {domainSymbolName} : IEntityDomain<{entityName}, {domainSymbolName}, {matcherName}, {aspectSetterName}>{domainComponentInterfaces}{domainIndexInterfaces}
    {{
        #if UNITY_EDITOR
        public Dictionary<long,{entityName}> EntitiesByUid;
        #endif

        private long _entitiesCreated;

        private readonly HashSet<{entityName}> _destroyedEntities;
        private readonly Dictionary<{matcherName}, CachingEntityGroup<{entityName}>> _cachingEntityGroups;
        private readonly Dictionary<{matcherName}, ReactiveEntityGroup<{entityName}>> _reactiveEntityGroups;

        private Aspect[] _aspects;

        private {entityName}[] _entities;

        public int CurrentCapacity {{ get; private set; }}
        public int CurrentEntityCount {{ get; private set; }}
        
        public ReadOnlySpan<{entityName}> Entities => CurrentEntityCount > 0
            ? new ReadOnlySpan<{entityName}>(_entities).Slice(0,CurrentEntityCount)
            : ReadOnlySpan<{entityName}>.Empty;

        public {matcherName} GetMatcher() => new ();

{domainComponents}
{componentArrays}
{indexStorage}
{observerStorage}
        public {domain.DomainName}(int initialEntityCapacity)
        {{
            Debug.Assert(initialEntityCapacity > 0,""Trying to create {domain.DomainName} instance with initial capacity less than one."");

            CurrentCapacity = initialEntityCapacity;
            CurrentEntityCount = 0;

            _entitiesCreated = 0;

            _entities = new {entityName}[CurrentCapacity];
            _aspects = new Aspect[CurrentCapacity];

{arraysInitialization}
            _destroyedEntities = new HashSet<{entityName}>();

            _cachingEntityGroups = new Dictionary<{matcherName}, CachingEntityGroup<{entityName}>>();
            _reactiveEntityGroups = new Dictionary<{matcherName}, ReactiveEntityGroup<{entityName}>>();

            #if UNITY_EDITOR
            EntitiesByUid = new Dictionary<long, {entityName}>();

            var container = ScriptableObject.CreateInstance<{domainSymbolName}Container>();
            container.{domainSymbolName} = this;
            #endif
        }}

        public {entityName} CreateEntity()
        {{
            if (CurrentEntityCount + 1 >= CurrentCapacity)
            {{
                DoubleCapacity();
            }}

            var entity = new {entityName}(this, CurrentEntityCount++, ++_entitiesCreated);
            _entities[entity.Index] = entity;
            _aspects[entity.Index] = CreateAspect();

            #if UNITY_EDITOR
            EntitiesByUid.Add(entity.UId,entity);
            #endif

            return entity;
        }}

        private void DoubleCapacity()
        {{
            //Double Capacity
            var newCapacity = CurrentCapacity * 2;

            var newEntities = new {entityName}[newCapacity];
            _entities.CopyTo(newEntities, 0);
            _entities = newEntities;

            var newAspects = new Aspect[newCapacity];
            _aspects.CopyTo(newAspects, 0);
            _aspects = newAspects;

{componentCapacityDoubling}
            CurrentCapacity = newCapacity;
        }}

        public void CleanupEntities()
        {{
            if(_destroyedEntities.Count == 0) return;

            var entities = new {domain.Entity.EntityTypeName}[_destroyedEntities.Count];
            _destroyedEntities.CopyTo(entities);
            Array.Sort(entities, (a,b) => b.Index.CompareTo(a.Index));
            foreach (var entity in entities)
            {{
                var index = entity.Index;
                CurrentEntityCount--;
{removeFromIndexes}
{removeObservers}
                if (index == CurrentEntityCount)
                {{
                    _entities[CurrentEntityCount] = default;
                    _aspects[CurrentEntityCount] = default;
                }}
                else
                {{
                    _entities[index] = new {entityName}(this, index, _entities[CurrentEntityCount].UId);
                    _aspects[index] = _aspects[CurrentEntityCount];
                    
                    _entities[CurrentEntityCount] = default;
                    _aspects[CurrentEntityCount] = default;
                    
                    ShiftComponents((int)CurrentEntityCount,(int)index);

{updateObserverEvents}
                    UpdateCachingGroupsWithMovedEntity(_entities[index]);
                    UpdateReactiveGroupsWithMovedEntity(_entities[index]);
{updateIndexesBlock}

                    #if UNITY_EDITOR
                    EntitiesByUid[_entities[index].UId] = _entities[index];
                    #endif
                }}
                #if UNITY_EDITOR
                EntitiesByUid.Remove(entity.UId);
                #endif
            }}
            _destroyedEntities.Clear();
        }}

        private void ShiftComponents(int from, int to)
        {{
{shiftComponentsBlock}
        }}

        public bool HasEntityBeenDestroyed({entityName} entity) =>
            CurrentEntityCount <= entity.Index && !_entities[entity.Index].Equals(entity);
        public void FlagForDestruction({entityName} entity)
        {{
            if(_destroyedEntities.Contains(entity) || HasEntityBeenDestroyed(entity)) return;
            _entities[entity.Index] = new {entityName}(this, 0, entity.UId);
            _destroyedEntities.Add(entity);
            UpdateCachingGroupsWithDestroyedEntity(_entities[entity.Index]);
            UpdateReactiveGroupsWithDestroyedEntity(_entities[entity.Index]);
        }}

        #region Aspects
        public Aspect CreateAspect() => new ({entityName}.AspectSize);
        public Aspect GetEntityAspect({entityName} entity) => HasEntityBeenDestroyed(entity) ? default : _aspects[entity.Index];

        private void SetAspect({entityName} entity, AspectIndex index)
        {{
            _aspects[entity.Index].Set(index);
            UpdateCachingGroupsWithChangedAspect(entity);
            UpdateReactiveGroupsWithChangedAspect(entity);
        }}

        private void RemoveAspect({entityName} entity, AspectIndex index)
        {{
            _aspects[entity.Index].Remove(index);
            UpdateCachingGroupsWithChangedAspect(entity);
            UpdateReactiveGroupsWithChangedAspect(entity);
        }}
        #endregion

        #region Groups
        public EntityGroup<{entityName},{domainSymbolName},{matcherName},{aspectSetterName}> GetEntityGroup({matcherName} matcher)
        {{
            return new (matcher, this);
        }}

        public CachingEntityGroup<{entityName}> GetCachingGroup({matcherName} matcher, bool shouldSort)
        {{
            matcher.RecalculateHash();
            if (_cachingEntityGroups.ContainsKey(matcher)) return _cachingEntityGroups[matcher];
            var group = new CachingEntityGroup<{entityName}>(matcher,Entities,shouldSort);
            _cachingEntityGroups.Add(matcher,group);
            return group;
        }}

        public ReactiveEntityGroup<{entityName}> GetReactiveGroup({matcherName} matcher, bool shouldSort)
        {{
            matcher.RecalculateHash();
            if (_reactiveEntityGroups.ContainsKey(matcher)) return _reactiveEntityGroups[matcher];
            var group = new ReactiveEntityGroup<{entityName}>(matcher,shouldSort);
            _reactiveEntityGroups.Add(matcher,group);
            return group;
        }}

        private void UpdateCachingGroupsWithChangedAspect({entityName} entity)
        {{
            foreach (var entityGroup in _cachingEntityGroups)
            {{
                entityGroup.Value.EntityAspectChanged(entity);
            }}
        }}

        private void UpdateReactiveGroupsWithChangedAspect({entityName} entity)
        {{
            foreach (var entityGroup in _reactiveEntityGroups)
            {{
                entityGroup.Value.EntityAspectChanged(entity);
            }}
        }}

        private void UpdateCachingGroupsWithMovedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _cachingEntityGroups)
            {{
                entityGroup.Value.EntityMoved(entity);
            }}
        }}

        private void UpdateReactiveGroupsWithMovedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _reactiveEntityGroups)
            {{
                entityGroup.Value.EntityMoved(entity);
            }}
        }}

        private void UpdateCachingGroupsWithDestroyedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _cachingEntityGroups)
            {{
                entityGroup.Value.EntityDestroyed(entity);
            }}
        }}

        private void UpdateReactiveGroupsWithDestroyedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _reactiveEntityGroups)
            {{
                entityGroup.Value.EntityDestroyed(entity);
            }}
        }}

        private void UpdateCachingGroupsWithValueChangedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _cachingEntityGroups)
            {{
                entityGroup.Value.EntityValueChanged(entity);
            }}
        }}

        private void UpdateReactiveGroupsWithValueChangedEntity({entityName} entity)
        {{
            foreach (var entityGroup in _reactiveEntityGroups)
            {{
                entityGroup.Value.EntityValueChanged(entity);
            }}
        }}
        #endregion
{indexGetters}
{componentFunctionBlock}
{indexBlocks}
{observerRegistration}
{observerPresenceChecks}
        public void TriggerEvent()
        {{
{observerTrigger}
        }}
    }}
}}
";
            FormattedFileWriter.WriteSourceFile(context,domainBlock,domainSymbolName);
        }

        private static string GenerateValueComponentCapacityDoublingBody(string componentName,
            ITypeSymbol componentType,
            string componentArrayName, StringBuilder componentCapacityDoubling)
        {
            var doubleComponentCapacity = $@"            var new{componentName} = new {componentType}[newCapacity];
            {componentArrayName}.CopyTo(new{componentName}, 0);
            {componentArrayName} = new{componentName};";
            componentCapacityDoubling.AppendLine(doubleComponentCapacity);
            var shiftComponent = $@"            {componentArrayName}[to] = {componentArrayName}[from];
            {componentArrayName}[from] = default;";
            return shiftComponent;
        }

        private static string GenerateEntityComponentObserverRegistrationBody(Domain domain, Component component)
        {
            if (component.ObserverScope == "Domain")
                return GenerateDomainLevelEntityComponentObserverRegistration(domain, component);
            else
                return GenerateEntityLevelEntityComponentObserverRegistration(domain, component);
        }

        private static string GenerateEntityLevelEntityComponentObserverRegistration(Domain domain, Component component)
        {
            return
                $@"        public {component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}> Observe{component.Name}({domain.Entity.EntityTypeName} entity, Action<{domain.Entity.EntityTypeName}, {component.Type}, ComponentEventType> handler)
        {{
            if(!_{component.Name.ToCamelCase()}Observers.ContainsKey(entity))
            {{
                _{component.Name.ToCamelCase()}Observers.Add(entity, new List<WeakReference<{component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>>>());
            }}
            var observerToken = new {component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>(handler);
            
            _{component.Name.ToCamelCase()}Observers[entity].Add(new WeakReference<{component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>>(observerToken));
            return observerToken;
        }}";
        }

        private static string GenerateDomainLevelEntityComponentObserverRegistration(Domain domain, Component component)
        {
            return
                $@"        public {component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}> Observe{component.Name}(Action<{domain.Entity.EntityTypeName}, {component.Type}, ComponentEventType> handler)
        {{
            var observerToken = new {component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>(handler);
            _{component.Name.ToCamelCase()}Observers.Add(new WeakReference<{component.Name}ObserverToken<{domain.Entity.EntityTypeName},{component.Type}>>(observerToken));
            return observerToken;
        }}";
        }
        
        private static string GenerateDomainComponentBody(Component domainComponent)
        {
            string componentBody;
            if (domainComponent.IsFlag)
            {
                componentBody =
                    $@"        public {domainComponent.Type} {domainComponent.Name} {{get; set;}}";
            }
            else
            {
                componentBody =
                    $@"        private {domainComponent.Type} _{domainComponent.Name.ToCamelCase()};        
        public bool Has{domainComponent.Name} {{get; private set;}}
        public {domainComponent.Type} {domainComponent.Name}
        {{
            get
            {{
                if(Has{domainComponent.Name})
                    return _{domainComponent.Name.ToCamelCase()};

                throw new InvalidOperationException();
            }}
            set
            {{
                Has{domainComponent.Name} = true;
                _{domainComponent.Name.ToCamelCase()} = value;
            }}
        }}
        public void Remove{domainComponent.Name}()
        {{
            Has{domainComponent.Name} = false;
            _{domainComponent.Name.ToCamelCase()} = default;
        }}
";
            }

            return componentBody;
        }

        private static string GenerateFlagComponentFunctionBlock(Component component, string componentName,
            string entityName,
            string componentAspectName, string componentArrayName, ITypeSymbol componentType)
        {
            var insertIndex = component.IsIndex ? $@"IndexBy{componentName}(entity);" : string.Empty;
            var removeIndex = component.IsIndex ? $@"RemoveFrom{componentName}Index(entity);" : string.Empty;
            var observedComponentValueChanged = component.IsObservable
                ? $@"if(!_{component.Name.ToCamelCase()}Events.ContainsKey(entity))
                {{
                    _{component.Name.ToCamelCase()}Events[entity] = ComponentEventType.ValueChanged;
                }}"
                : string.Empty;
            var componentFunctions =
                $@"        public bool Has{componentName}({entityName} entity) => _aspects[entity.Index].ContainsComponent({entityName}.{componentAspectName});
        public void Set{componentName}({entityName} entity, {componentType.Name} value)
        {{
            if(value == Has{componentName}(entity)) return;
            {observedComponentValueChanged}
            {removeIndex}
            if(value)
            {{
                SetAspect(entity, {entityName}.{component.AspectName});
            }}
            else
            {{
                RemoveAspect(entity, {entityName}.{component.AspectName});
            }}
            {insertIndex}
        }}";
            return componentFunctions;
        }

        private static string GenerateComponentFunctionBlock(Component component, string componentName,
            string entityName,
            string componentAspectName, string componentArrayName, ITypeSymbol componentType)
        {
            var insertIndex = component.IsIndex ? $@"IndexBy{componentName}(entity);" : string.Empty;
            var removeIndex = component.IsIndex ? $@"RemoveFrom{componentName}Index(entity);" : string.Empty;
            var observedComponentAdded = component.IsObservable
                ? $@"if(Has{component.Name}Observers(entity))
                {{
                    if(_{component.Name.ToCamelCase()}Events.ContainsKey(entity))
                    {{
                        // Component removed since the last event tick replacing with changed.
                        _{component.Name.ToCamelCase()}Events[entity] = ComponentEventType.ValueChanged;
                    }}
                    else
                    {{
                        _{component.Name.ToCamelCase()}Events.Add(entity, ComponentEventType.Added);
                    }}
                }}"
                : string.Empty;
            var observedComponentRemoved = component.IsObservable
                ? $@"if(Has{component.Name}Observers(entity))
                {{
                    if(_{component.Name.ToCamelCase()}Events.ContainsKey(entity))
                    {{
                        if(_{component.Name.ToCamelCase()}Events[entity] == ComponentEventType.Added)
                        {{
                            // Component both added and removed since the last tick removing the event.
                            _{component.Name.ToCamelCase()}Events.Remove(entity);
                        }}
                        else
                        {{
                            _{component.Name.ToCamelCase()}Events[entity] = ComponentEventType.Removed;
                        }}
                    }}
                    else
                    {{
                        _{component.Name.ToCamelCase()}Events.Add(entity, ComponentEventType.Removed);
                    }}
                }}"
                : string.Empty;
            var observedComponentChanged = component.IsObservable
                ? $@"if(Has{component.Name}Observers(entity))
                {{
                    if(!_{component.Name.ToCamelCase()}Events.ContainsKey(entity))
                    {{
                        _{component.Name.ToCamelCase()}Events.Add(entity, ComponentEventType.ValueChanged);
                    }}
                }}"
                : string.Empty;
            var componentFunctions =
                $@"        public bool Has{componentName}({entityName} entity) => _aspects[entity.Index].ContainsComponent({entityName}.{componentAspectName});

        public {componentType} Get{componentName}({entityName} entity)
        {{
                if(!HasEntityBeenDestroyed(entity) && Has{componentName}(entity))
                    return {componentArrayName}[entity.Index];

                throw new InvalidOperationException();
        }}

        public void Add{componentName}({entityName} entity, in {componentType} value)
        {{
                if (HasEntityBeenDestroyed(entity) || Has{componentName}(entity))
                    throw new InvalidOperationException();

                SetAspect(entity, {entityName}.{componentAspectName});
                {componentArrayName}[entity.Index] = value;
                {insertIndex}
                {observedComponentAdded}
        }}

        public void Replace{componentName}({entityName} entity, in {componentType} value)
        {{
                if (HasEntityBeenDestroyed(entity) || !Has{componentName}(entity))
                    throw new InvalidOperationException();
    
                {removeIndex}
                {componentArrayName}[entity.Index] = value;
                {insertIndex}
                {observedComponentChanged}
                UpdateCachingGroupsWithValueChangedEntity(entity);
                UpdateReactiveGroupsWithValueChangedEntity(entity);
        }}

        public void Remove{componentName}({entityName} entity)
        {{
                if (HasEntityBeenDestroyed(entity) || !Has{componentName}(entity))
                    throw new InvalidOperationException();
    
                {removeIndex}
                {observedComponentRemoved}
                RemoveAspect(entity, {entityName}.{componentAspectName});
                {componentArrayName}[entity.Index] = default;
        }}";
            return componentFunctions;
        }
    }
}