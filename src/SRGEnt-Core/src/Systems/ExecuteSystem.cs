using System;
using SRGEnt.Groups;
using SRGEnt.Interfaces;

namespace SRGEnt.Systems
{
    public abstract class ExecuteSystem<TEntity, TDomain, TMatcher, TAspectSetter> : ISystem 
        where TEntity : struct, IEntity, IEquatable<TEntity> 
        where TDomain : IEntityDomain<TEntity,TDomain, TMatcher,TAspectSetter>
        where TMatcher : IAspectMatcher<TAspectSetter>
        where TAspectSetter : IAspectMatcher<TAspectSetter>
    {
        protected readonly TDomain _domain;
        private readonly CachingEntityGroup<TEntity> _internalGroup;

        public ExecuteSystem(TDomain domain, bool shouldSort = false)
        {
            var matcher = domain.GetMatcher();
            _domain = domain;
            SetMatcher(ref matcher);
            _internalGroup = _domain.GetCachingGroup(matcher, shouldSort);
        }

        public void Execute()
        {
            Execute(_internalGroup.Entities);
            _domain.CleanupEntities();
        }

        protected abstract void SetMatcher(ref TMatcher matcher);

        protected abstract void Execute(ReadOnlySpan<TEntity> entities);
    }
}
