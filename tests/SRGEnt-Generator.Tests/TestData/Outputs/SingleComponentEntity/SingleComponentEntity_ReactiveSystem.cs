﻿// This file have been generated by SRGEnt-Generator
using SRGEnt.Systems;

namespace SRGEnt.Generated
{
    public abstract class SingleComponentTestReactiveSystem : ReactiveSystem<SingleComponentTestEntity, SingleComponentTestDomain, SingleComponentTestMatcher, SingleComponentTestAspectSetter>
    {
        protected SingleComponentTestReactiveSystem(SingleComponentTestDomain domain, bool shouldSort = false) : base(domain, shouldSort)
        {
        }
    }
}