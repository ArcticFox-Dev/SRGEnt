using System;
using System.Collections.Generic;
using SRGEnt.Aspects;
using SRGEnt.Interfaces;
using SRGEnt.Groups.Utils;

namespace SRGEnt.Groups
{
    public class ReactiveEntityGroup<TEntity>: IEntityGroup<TEntity>, IDisposable 
        where TEntity: struct, IEntity, IEquatable<TEntity>
    {
        private static readonly EntityIndexComparer<TEntity> _comparer = new EntityIndexComparer<TEntity>();
        
        private readonly AspectMatcher _matcher;
        private readonly bool _shouldSort;

        private TEntity[] _matchingEntities;
        private int _matchingEntitiesCount;

        private readonly HashSet<TEntity> _changedEntities = new HashSet<TEntity>();
        private readonly HashSet<TEntity> _movedEntities = new HashSet<TEntity>();
        private readonly HashSet<TEntity> _destroyedEntities = new HashSet<TEntity>();

        public ReactiveEntityGroup(AspectMatcher matcher, bool shouldSort = false)
        {
            _matcher = matcher;
            _shouldSort = shouldSort;
            _matchingEntities = new TEntity[50];
        }

        public void Dispose()
        {}
        
        public ReadOnlySpan<TEntity> Entities
        {
            get
            {
                UpdateMovedEntities();
                RemoveEntities();
                
                _matchingEntitiesCount = 0;
                foreach (var entity in _changedEntities)
                {
                    if(_matcher.MatchesAspect(entity.Aspect))
                    {
                        AddMatchingEntity(entity);
                    }
                }
                _changedEntities.Clear();

                if(_shouldSort)
                {
                    Array.Sort(_matchingEntities,0,_matchingEntitiesCount, _comparer);
                }
                return new ReadOnlySpan<TEntity>(_matchingEntities).Slice(0,_matchingEntitiesCount);
            }
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
        {
            _changedEntities.Add(entity);
        }
        
        private void UpdateMovedEntities()
        {
            foreach (var entity in _movedEntities)
            {
                if (_changedEntities.Contains(entity))
                {
                    // This needs to be done as changed entities might contain an old index
                    _changedEntities.Remove(entity);
                    _changedEntities.Add(entity);
                }
            }
            _movedEntities.Clear();
        }
        
        private void RemoveEntities()
        {
            foreach (var entity in _destroyedEntities)
            {
                if (_changedEntities.Contains(entity))
                {
                    _changedEntities.Remove(entity);
                }
            }
            _destroyedEntities.Clear();
        }

        private void AddMatchingEntity(TEntity entity)
        {
            if (_matchingEntitiesCount + 1 == _matchingEntities.Length)
            {
                var newArray = new TEntity[_matchingEntities.Length * 2];
                new ReadOnlySpan<TEntity>(_matchingEntities).CopyTo(newArray);
                _matchingEntities = newArray;
            }
            _matchingEntities[_matchingEntitiesCount++] = entity;
        }
    }
}