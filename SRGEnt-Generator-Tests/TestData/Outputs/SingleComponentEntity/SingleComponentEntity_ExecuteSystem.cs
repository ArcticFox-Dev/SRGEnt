﻿// This file have been generated by SRGEnt-Generator
using SRGEnt.Systems;

namespace SRGEnt.Generated
{
    public abstract class SingleComponentTestExecuteSystem : ExecuteSystem<SingleComponentTestEntity, SingleComponentTestDomain, SingleComponentTestMatcher, SingleComponentTestAspectSetter>
    {
        protected SingleComponentTestExecuteSystem(SingleComponentTestDomain domain, bool shouldSort = false) : base(domain, shouldSort)
        {
        }
    }
}