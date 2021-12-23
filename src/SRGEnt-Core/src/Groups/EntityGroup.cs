using System;
using SRGEnt.Aspects;
using SRGEnt.Interfaces;

namespace SRGEnt.Groups
{
    public class EntityGroup<TEntity, TDomain, TMatcher, TAspectSetter> : IEntityGroup<TEntity>
        where TEntity : struct, IEntity, IEquatable<TEntity>
        where TDomain : IEntityDomain<TEntity, TDomain, TMatcher, TAspectSetter>
        where TMatcher : IAspectMatcher<TAspectSetter>
        where TAspectSetter : IAspectMatcher<TAspectSetter>
    {
        private readonly AspectMatcher _matcher;
        private readonly TDomain _componentManager;

        public EntityGroup(AspectMatcher matcher, TDomain componentManager)
        {
            _matcher = matcher;
            _componentManager = componentManager;
        }

        public ReadOnlySpan<TEntity> Entities
        {
            get
            {
                var entities = _componentManager.Entities;
                var filteredEntities = new TEntity[_componentManager.CurrentEntityCount];

                var matchedEntities = 0;
                for (var i = 0; i < entities.Length; i++)
                {
                    if (entities[i].HasBeenDestroyed) continue;

                    var aspect = entities[i].Aspect;
                    if (_matcher.MatchesAspect(aspect))
                    {
                        filteredEntities[matchedEntities++] = entities[i];
                    }
                }

                return new ReadOnlySpan<TEntity>(filteredEntities).Slice(0, matchedEntities);
            }
        }

        public void EntityDestroyed(TEntity entity)
        {
        }

        public void EntityAspectChanged(TEntity entity)
        {
        }

        public void EntityMoved(TEntity entity)
        {
        }

        public void EntityValueChanged(TEntity entity)
        {
        }
    }
}
