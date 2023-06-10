// This file have been generated by SRGEnt-Generator
using System;
using System.Collections.Generic;
using SRGEnt.Interfaces;
using SRGEnt.Groups;
using SRGEnt.Aspects;
using System.Diagnostics;

namespace SRGEnt.Generated
{
    public partial class PrimaryIndexComponentTestDomain : IEntityDomain<PrimaryIndexComponentTestEntity, PrimaryIndexComponentTestDomain, PrimaryIndexComponentTestMatcher, PrimaryIndexComponentTestAspectSetter>, IFirstComponentPrimaryIndex<PrimaryIndexComponentTestEntity, int>, ISecondComponentPrimaryIndex<PrimaryIndexComponentTestEntity, int>
    {
        private long _entitiesCreated;
        private readonly HashSet<PrimaryIndexComponentTestEntity> _destroyedEntities;
        private readonly Dictionary<PrimaryIndexComponentTestMatcher, CachingEntityGroup<PrimaryIndexComponentTestEntity>> _cachingEntityGroups;
        private readonly Dictionary<PrimaryIndexComponentTestMatcher, ReactiveEntityGroup<PrimaryIndexComponentTestEntity>> _reactiveEntityGroups;
        private Aspect[] _aspects;
        private PrimaryIndexComponentTestEntity[] _entities;
        public int CurrentCapacity { get; private set; }

        public int CurrentEntityCount { get; private set; }

        public ReadOnlySpan<PrimaryIndexComponentTestEntity> Entities => CurrentEntityCount > 0 ? new ReadOnlySpan<PrimaryIndexComponentTestEntity>(_entities).Slice(0, CurrentEntityCount) : ReadOnlySpan<PrimaryIndexComponentTestEntity>.Empty;
        public PrimaryIndexComponentTestMatcher GetMatcher() => new();
        private int[] _firstComponentComponents;
        private int[] _secondComponentComponents;
        private Dictionary<int, PrimaryIndexComponentTestEntity> _firstComponentIndex = new();
        private Dictionary<int, PrimaryIndexComponentTestEntity> _secondComponentIndex = new();
        public PrimaryIndexComponentTestDomain(int initialEntityCapacity)
        {
            Debug.Assert(initialEntityCapacity > 0, "Trying to create PrimaryIndexComponentTestDomain instance with initial capacity less than one.");
            CurrentCapacity = initialEntityCapacity;
            CurrentEntityCount = 0;
            _entitiesCreated = 0;
            _entities = new PrimaryIndexComponentTestEntity[CurrentCapacity];
            _aspects = new Aspect[CurrentCapacity];
            _firstComponentComponents = new int[CurrentCapacity];
            _secondComponentComponents = new int[CurrentCapacity];
            _destroyedEntities = new HashSet<PrimaryIndexComponentTestEntity>();
            _cachingEntityGroups = new Dictionary<PrimaryIndexComponentTestMatcher, CachingEntityGroup<PrimaryIndexComponentTestEntity>>();
            _reactiveEntityGroups = new Dictionary<PrimaryIndexComponentTestMatcher, ReactiveEntityGroup<PrimaryIndexComponentTestEntity>>();
            //ConstructorExtensionLateHook();
        }

        //private partial void ConstructorExtensionLateHook();
        public PrimaryIndexComponentTestEntity CreateEntity()
        {
            if (CurrentEntityCount + 1 >= CurrentCapacity)
            {
                DoubleCapacity();
            }

            var entity = new PrimaryIndexComponentTestEntity(this, CurrentEntityCount++, ++_entitiesCreated);
            _entities[entity.Index] = entity;
            _aspects[entity.Index] = CreateAspect();
            //CreateEntityExtensionLateHook(entity);
            return entity;
        }

        //private partial void CreateEntityExtensionLateHook(PrimaryIndexComponentTestEntity entity);
        private void DoubleCapacity()
        {
            //Double Capacity
            var newCapacity = CurrentCapacity * 2;
            var newEntities = new PrimaryIndexComponentTestEntity[newCapacity];
            _entities.CopyTo(newEntities, 0);
            _entities = newEntities;
            var newAspects = new Aspect[newCapacity];
            _aspects.CopyTo(newAspects, 0);
            _aspects = newAspects;
            var newFirstComponent = new int[newCapacity];
            _firstComponentComponents.CopyTo(newFirstComponent, 0);
            _firstComponentComponents = newFirstComponent;
            var newSecondComponent = new int[newCapacity];
            _secondComponentComponents.CopyTo(newSecondComponent, 0);
            _secondComponentComponents = newSecondComponent;
            CurrentCapacity = newCapacity;
        }

        public void CleanupEntities()
        {
            if (_destroyedEntities.Count == 0)
                return;
            var entities = new PrimaryIndexComponentTestEntity[_destroyedEntities.Count];
            _destroyedEntities.CopyTo(entities);
            Array.Sort(entities, (a, b) => b.Index.CompareTo(a.Index));
            foreach (var entity in entities)
            {
                var index = entity.Index;
                CurrentEntityCount--;
                RemoveFromFirstComponentIndex(entity);
                RemoveFromSecondComponentIndex(entity);
                if (index == CurrentEntityCount)
                {
                    _entities[CurrentEntityCount] = default;
                    _aspects[CurrentEntityCount] = default;
                }
                else
                {
                    _entities[index] = new PrimaryIndexComponentTestEntity(this, index, _entities[CurrentEntityCount].UId);
                    _aspects[index] = _aspects[CurrentEntityCount];
                    _entities[CurrentEntityCount] = default;
                    _aspects[CurrentEntityCount] = default;
                    ShiftComponents((int)CurrentEntityCount, (int)index);
                    UpdateCachingGroupsWithMovedEntity(_entities[index]);
                    UpdateReactiveGroupsWithMovedEntity(_entities[index]);
                    if (HasFirstComponent(entity))
                    {
                        RemoveFromFirstComponentIndex(_entities[index]);
                        IndexByFirstComponent(_entities[index]);
                    }

                    if (HasSecondComponent(entity))
                    {
                        RemoveFromSecondComponentIndex(_entities[index]);
                        IndexBySecondComponent(_entities[index]);
                    }

                    //EntityMovedExtensionLateHook(index);
                }

                //EntityRemovedExtensionLateHook(entity);
            }

            _destroyedEntities.Clear();
        }

        //private partial void EntityMovedExtensionLateHook(int index);
        //private partial void EntityRemovedExtensionLateHook(PrimaryIndexComponentTestEntity entity);
        private void ShiftComponents(int from, int to)
        {
            _firstComponentComponents[to] = _firstComponentComponents[from];
            _firstComponentComponents[from] = default;
            _secondComponentComponents[to] = _secondComponentComponents[from];
            _secondComponentComponents[from] = default;
        }

        public bool HasEntityBeenDestroyed(PrimaryIndexComponentTestEntity entity) => CurrentEntityCount <= entity.Index && !_entities[entity.Index].Equals(entity);
        public void FlagForDestruction(PrimaryIndexComponentTestEntity entity)
        {
            if (_destroyedEntities.Contains(entity) || HasEntityBeenDestroyed(entity))
                return;
            _entities[entity.Index] = new PrimaryIndexComponentTestEntity(this, 0, entity.UId);
            _destroyedEntities.Add(entity);
            UpdateCachingGroupsWithDestroyedEntity(_entities[entity.Index]);
            UpdateReactiveGroupsWithDestroyedEntity(_entities[entity.Index]);
        }

#region Aspects
        public Aspect CreateAspect() => new(PrimaryIndexComponentTestEntity.AspectSize);
        public Aspect GetEntityAspect(PrimaryIndexComponentTestEntity entity) => HasEntityBeenDestroyed(entity) ? default : _aspects[entity.Index];
        private void SetAspect(PrimaryIndexComponentTestEntity entity, AspectIndex index)
        {
            _aspects[entity.Index].Set(index);
            UpdateCachingGroupsWithChangedAspect(entity);
            UpdateReactiveGroupsWithChangedAspect(entity);
        }

        private void RemoveAspect(PrimaryIndexComponentTestEntity entity, AspectIndex index)
        {
            _aspects[entity.Index].Remove(index);
            UpdateCachingGroupsWithChangedAspect(entity);
            UpdateReactiveGroupsWithChangedAspect(entity);
        }

#endregion
#region Groups
        public EntityGroup<PrimaryIndexComponentTestEntity, PrimaryIndexComponentTestDomain, PrimaryIndexComponentTestMatcher, PrimaryIndexComponentTestAspectSetter> GetEntityGroup(PrimaryIndexComponentTestMatcher matcher)
        {
            return new(matcher, this);
        }

        public CachingEntityGroup<PrimaryIndexComponentTestEntity> GetCachingGroup(PrimaryIndexComponentTestMatcher matcher, bool shouldSort)
        {
            matcher.RecalculateHash();
            if (_cachingEntityGroups.ContainsKey(matcher))
                return _cachingEntityGroups[matcher];
            var group = new CachingEntityGroup<PrimaryIndexComponentTestEntity>(matcher, Entities, shouldSort);
            _cachingEntityGroups.Add(matcher, group);
            return group;
        }

        public ReactiveEntityGroup<PrimaryIndexComponentTestEntity> GetReactiveGroup(PrimaryIndexComponentTestMatcher matcher, bool shouldSort)
        {
            matcher.RecalculateHash();
            if (_reactiveEntityGroups.ContainsKey(matcher))
                return _reactiveEntityGroups[matcher];
            var group = new ReactiveEntityGroup<PrimaryIndexComponentTestEntity>(matcher, shouldSort);
            _reactiveEntityGroups.Add(matcher, group);
            return group;
        }

        private void UpdateCachingGroupsWithChangedAspect(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _cachingEntityGroups)
            {
                entityGroup.Value.EntityAspectChanged(entity);
            }
        }

        private void UpdateReactiveGroupsWithChangedAspect(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _reactiveEntityGroups)
            {
                entityGroup.Value.EntityAspectChanged(entity);
            }
        }

        private void UpdateCachingGroupsWithMovedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _cachingEntityGroups)
            {
                entityGroup.Value.EntityMoved(entity);
            }
        }

        private void UpdateReactiveGroupsWithMovedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _reactiveEntityGroups)
            {
                entityGroup.Value.EntityMoved(entity);
            }
        }

        private void UpdateCachingGroupsWithDestroyedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _cachingEntityGroups)
            {
                entityGroup.Value.EntityDestroyed(entity);
            }
        }

        private void UpdateReactiveGroupsWithDestroyedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _reactiveEntityGroups)
            {
                entityGroup.Value.EntityDestroyed(entity);
            }
        }

        private void UpdateCachingGroupsWithValueChangedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _cachingEntityGroups)
            {
                entityGroup.Value.EntityValueChanged(entity);
            }
        }

        private void UpdateReactiveGroupsWithValueChangedEntity(PrimaryIndexComponentTestEntity entity)
        {
            foreach (var entityGroup in _reactiveEntityGroups)
            {
                entityGroup.Value.EntityValueChanged(entity);
            }
        }

#endregion
        public PrimaryIndexComponentTestEntity? GetEntityWithFirstComponent(int value)
        {
            if (!_firstComponentIndex.ContainsKey(value))
                return null;
            return _firstComponentIndex[value];
        }

        public PrimaryIndexComponentTestEntity? GetEntityWithSecondComponent(int value)
        {
            if (!_secondComponentIndex.ContainsKey(value))
                return null;
            return _secondComponentIndex[value];
        }

        public bool HasFirstComponent(PrimaryIndexComponentTestEntity entity) => _aspects[entity.Index].ContainsComponent(PrimaryIndexComponentTestEntity.FIRSTCOMPONENT_INDEX);
        public int GetFirstComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (!HasEntityBeenDestroyed(entity) && HasFirstComponent(entity))
                return _firstComponentComponents[entity.Index];
            throw new InvalidOperationException();
        }

        public void AddFirstComponent(PrimaryIndexComponentTestEntity entity, in int value)
        {
            if (HasEntityBeenDestroyed(entity) || HasFirstComponent(entity))
                throw new InvalidOperationException();
            SetAspect(entity, PrimaryIndexComponentTestEntity.FIRSTCOMPONENT_INDEX);
            _firstComponentComponents[entity.Index] = value;
            IndexByFirstComponent(entity);
        }

        public void ReplaceFirstComponent(PrimaryIndexComponentTestEntity entity, in int value)
        {
            if (HasEntityBeenDestroyed(entity) || !HasFirstComponent(entity))
                throw new InvalidOperationException();
            RemoveFromFirstComponentIndex(entity);
            _firstComponentComponents[entity.Index] = value;
            IndexByFirstComponent(entity);
            UpdateCachingGroupsWithValueChangedEntity(entity);
            UpdateReactiveGroupsWithValueChangedEntity(entity);
        }

        public void RemoveFirstComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (HasEntityBeenDestroyed(entity) || !HasFirstComponent(entity))
                throw new InvalidOperationException();
            RemoveFromFirstComponentIndex(entity);
            RemoveAspect(entity, PrimaryIndexComponentTestEntity.FIRSTCOMPONENT_INDEX);
            _firstComponentComponents[entity.Index] = default;
        }

        public bool HasSecondComponent(PrimaryIndexComponentTestEntity entity) => _aspects[entity.Index].ContainsComponent(PrimaryIndexComponentTestEntity.SECONDCOMPONENT_INDEX);
        public int GetSecondComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (!HasEntityBeenDestroyed(entity) && HasSecondComponent(entity))
                return _secondComponentComponents[entity.Index];
            throw new InvalidOperationException();
        }

        public void AddSecondComponent(PrimaryIndexComponentTestEntity entity, in int value)
        {
            if (HasEntityBeenDestroyed(entity) || HasSecondComponent(entity))
                throw new InvalidOperationException();
            SetAspect(entity, PrimaryIndexComponentTestEntity.SECONDCOMPONENT_INDEX);
            _secondComponentComponents[entity.Index] = value;
            IndexBySecondComponent(entity);
        }

        public void ReplaceSecondComponent(PrimaryIndexComponentTestEntity entity, in int value)
        {
            if (HasEntityBeenDestroyed(entity) || !HasSecondComponent(entity))
                throw new InvalidOperationException();
            RemoveFromSecondComponentIndex(entity);
            _secondComponentComponents[entity.Index] = value;
            IndexBySecondComponent(entity);
            UpdateCachingGroupsWithValueChangedEntity(entity);
            UpdateReactiveGroupsWithValueChangedEntity(entity);
        }

        public void RemoveSecondComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (HasEntityBeenDestroyed(entity) || !HasSecondComponent(entity))
                throw new InvalidOperationException();
            RemoveFromSecondComponentIndex(entity);
            RemoveAspect(entity, PrimaryIndexComponentTestEntity.SECONDCOMPONENT_INDEX);
            _secondComponentComponents[entity.Index] = default;
        }

        private void IndexByFirstComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (_firstComponentIndex.ContainsKey(entity.FirstComponent))
            {
                throw new ArgumentException("Trying to set duplicate PrimaryIndex key");
            }

            _firstComponentIndex.Add(entity.FirstComponent, entity);
        }

        private void RemoveFromFirstComponentIndex(PrimaryIndexComponentTestEntity entity)
        {
            if (!entity.HasFirstComponent)
                return;
            _firstComponentIndex.Remove(entity.FirstComponent);
        }

        private void IndexBySecondComponent(PrimaryIndexComponentTestEntity entity)
        {
            if (_secondComponentIndex.ContainsKey(entity.SecondComponent))
            {
                throw new ArgumentException("Trying to set duplicate PrimaryIndex key");
            }

            _secondComponentIndex.Add(entity.SecondComponent, entity);
        }

        private void RemoveFromSecondComponentIndex(PrimaryIndexComponentTestEntity entity)
        {
            if (!entity.HasSecondComponent)
                return;
            _secondComponentIndex.Remove(entity.SecondComponent);
        }

        public void TriggerEvent()
        {
        }
    }
}