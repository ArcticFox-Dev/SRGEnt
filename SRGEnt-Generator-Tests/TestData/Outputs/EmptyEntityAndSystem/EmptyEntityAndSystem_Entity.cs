﻿// This file have been generated by SRGEnt-Generator
using System;
using SRGEnt.Interfaces;
using SRGEnt.Aspects;
//using SRGEnt.Enums;

namespace SRGEnt.Generated
{
    public partial struct TestEntity : IEntity, IEquatable<TestEntity>
    {
        public const int ComponentCount = 0;
        public const int AspectSize = 1;
        private readonly TestDomain _domain;
        public TestEntity(TestDomain domain, int index, long uId)
        {
            _domain = domain;
            UId = uId;
            Index = index;
        }

        public readonly long UId { get; }

        public readonly int Index { get; }

        public readonly bool HasBeenDestroyed => _domain.HasEntityBeenDestroyed(this);
        public readonly Aspect Aspect => _domain.GetEntityAspect(this);
        public bool Equals(TestEntity other)
        {
            return UId == other.UId;
        }

        public override bool Equals(object obj)
        {
            return obj is TestEntity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return UId.GetHashCode();
        }
    }
}