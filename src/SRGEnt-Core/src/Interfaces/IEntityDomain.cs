using System;
using SRGEnt.Aspects;
using SRGEnt.Groups;

namespace SRGEnt.Interfaces
{
    public interface IEntityDomain<TEntity, TDomain, TMatcher, TAspectSetter> 
        where TEntity : struct, IEntity, IEquatable<TEntity>
        where TDomain : IEntityDomain<TEntity,TDomain,TMatcher,TAspectSetter>
        where TMatcher : IAspectMatcher<TAspectSetter> 
        where TAspectSetter : IAspectMatcher<TAspectSetter>
    {
        // ---------------
        // Inspection APIs
        // ---------------
        int CurrentCapacity { get; }
        int CurrentEntityCount { get; }
        
        // -----------
        // Entity APIs
        // -----------
        TEntity CreateEntity();
        void FlagForDestruction(TEntity entity);
        bool HasEntityBeenDestroyed(TEntity entity);
        ReadOnlySpan<TEntity> Entities { get; }
        void CleanupEntities();
        
        // ----------
        // Group APIs
        // ----------
        EntityGroup<TEntity,TDomain,TMatcher,TAspectSetter> GetEntityGroup(TMatcher matcher);
        CachingEntityGroup<TEntity> GetCachingGroup(TMatcher matcher, bool sortEntities);
        ReactiveEntityGroup<TEntity> GetReactiveGroup(TMatcher matcher, bool sortEntities);
        
        // ------------
        // Aspect APIs
        // ------------
        Aspect CreateAspect();
        TMatcher GetMatcher();
    }
}
