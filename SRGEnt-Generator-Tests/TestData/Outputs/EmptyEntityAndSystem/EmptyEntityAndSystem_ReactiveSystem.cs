﻿// This file have been generated by SRGEnt-Generator
using SRGEnt.Systems;

namespace SRGEnt.Generated
{
    public abstract class TestDomainReactiveSystem : ReactiveSystem<TestEntity, TestDomain, TestMatcher, TestAspectSetter>
    {
        protected TestDomainReactiveSystem(TestDomain domain, bool shouldSort = false) : base(domain, shouldSort)
        {
        }
    }
}