using System;
using System.Collections.Generic;
using SRGEnt.Aspects;
using SRGEnt.Interfaces;
using SRGEnt.Groups.Utils;

namespace SRGEnt.Groups
{
    public class CachingEntityGroup<TEntity>: ICachingEntityGroup<TEntity>, IDisposable 
        where TEntity: struct, IEntity, IEquatable<TEntity>
    {
        private readonly AspectMatcher _matcher;

        private TEntity[] _matchingEntities;

        private readonly HashSet<TEntity> _changedEntities = new HashSet<TEntity>();
        private readonly HashSet<TEntity> _movedEntities = new HashSet<TEntity>();
        private readonly HashSet<TEntity> _destroyedEntities = new HashSet<TEntity>();

        private static readonly EntityIndexComparer<TEntity> _comparer = new EntityIndexComparer<TEntity>();

        public CachingEntityGroup(AspectMatcher matcher, ReadOnlySpan<TEntity> entities)
        {
            _matcher = matcher;
            PrepopulateEntities(entities);
        }

        public void Dispose()
        { }

        #region IEntityGroup Implementation

        public ReadOnlySpan<TEntity> Entities
        {
            get
            {
                UpdateCachedEntities();
                return GetMatchingEntities();
            }
        }

        #endregion

        #region ICachingGroupImplementation

        public int EntityCount { get; private set; }

        public void UpdateCachedEntities()
        {
            if (_movedEntities.Count > 0 || _changedEntities.Count > 0 || _destroyedEntities.Count > 0)
            {
                UpdateMovedEntities();
                UpdateChangedEntities();
                RemoveEntities();
                Array.Sort(_matchingEntities, 0, EntityCount, _comparer);
            }
        }

        public ReadOnlySpan<TEntity> GetCachedEntities(bool update)
        {
            return update ? Entities : GetMatchingEntities();
        }

        public void EntityDestroyed(TEntity entity)
        {
            _destroyedEntities.Add(entity);
        }

        public void EntityAspectChanged(TEntity entity)
        {
            _changedEntities.Add(entity);
        }

        public void EntityMoved(TEntity entity)
        {
            _movedEntities.Remove(entity);
            _movedEntities.Add(entity);
        }

        public void EntityValueChanged(TEntity entity)
        { }

        #endregion
        
        private ReadOnlySpan<TEntity> GetMatchingEntities()
        {
            return new ReadOnlySpan<TEntity>(_matchingEntities).Slice(0, EntityCount);
        }

        private void PrepopulateEntities(ReadOnlySpan<TEntity> entities)
        {
            _matchingEntities = new TEntity[Math.Max(entities.Length, 50)];

            for (var i = 0; i < entities.Length; i++)
            {
                var aspect = entities[i].Aspect;
                if (_matcher.MatchesAspect(aspect))
                {
                    AddMatchingEntity(entities[i]);
                }
            }
        }

        private void AddMatchingEntity(TEntity entity)
        {
            if (EntityCount + 1 == _matchingEntities.Length)
            {
                var newArray = new TEntity[_matchingEntities.Length * 2];
                new ReadOnlySpan<TEntity>(_matchingEntities).CopyTo(newArray);
                _matchingEntities = newArray;
            }
            _matchingEntities[EntityCount++] = entity;
        }

        private void UpdateMovedEntities()
        {
            foreach (var entity in _movedEntities)
            {
                for (int i = 0; i < EntityCount; i++)
                {
                    if (_matchingEntities[i].Equals(entity))
                    {
                        _matchingEntities[i] = entity;
                    }
                }

                if (_changedEntities.Contains(entity))
                {
                    // This needs to be done as the entities to update might contain an old index
                    _changedEntities.Remove(entity);
                    _changedEntities.Add(entity);
                }
            }
            _movedEntities.Clear();
        }

        private void UpdateChangedEntities()
        {
            foreach (var entity in _changedEntities)
            {
                if(entity.HasBeenDestroyed) continue;
                var matching = _matcher.MatchesAspect(entity.Aspect);

                var contained = false;
                for (int i = 0; i < EntityCount; i++)
                {
                    if (_matchingEntities[i].Equals(entity))
                    {
                        contained = true;
                        if (!matching)
                        {
                            _destroyedEntities.Add(entity);
                        }
                    }
                }
                if (matching && !contained)
                {
                    AddMatchingEntity(entity);
                }
            }
            _changedEntities.Clear();
        }

        private void RemoveEntities()
        {
            foreach (var entity in _destroyedEntities)
            {
                for (var i = 0; i < EntityCount; i++)
                {
                    if (_matchingEntities[i].Equals(entity))
                    {
                        if (i == EntityCount - 1)
                        {
                            _matchingEntities[i] = default;
                            EntityCount--;
                            break;
                        }
                        Array.Copy(_matchingEntities,i+1,_matchingEntities, i, _matchingEntities.Length - i - 1);
                        EntityCount--;
                        _matchingEntities[EntityCount] = default;
                        break;
                    }
                }
            }
            _destroyedEntities.Clear();
        }
    }
}