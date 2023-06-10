// This file have been generated by SRGEnt-Generator
using System;
using SRGEnt.Interfaces;
using SRGEnt.Aspects;
//using SRGEnt.Enums;

namespace SRGEnt.Generated
{
    public partial struct PrimaryIndexComponentTestEntity : IEntity, IEquatable<PrimaryIndexComponentTestEntity>, IFirstComponent<int>, ISecondComponent<int>
    {
        public const int FIRSTCOMPONENT_INDEX = 0;
        public const int SECONDCOMPONENT_INDEX = 1;
        public const int ComponentCount = 2;
        public const int AspectSize = 1;
        private readonly PrimaryIndexComponentTestDomain _domain;
        public PrimaryIndexComponentTestEntity(PrimaryIndexComponentTestDomain domain, int index, long uId)
        {
            _domain = domain;
            UId = uId;
            Index = index;
        }

        public readonly long UId { get; }

        public readonly int Index { get; }

        public readonly bool HasBeenDestroyed => _domain.HasEntityBeenDestroyed(this);
        public readonly Aspect Aspect => _domain.GetEntityAspect(this);
        public bool Equals(PrimaryIndexComponentTestEntity other)
        {
            return UId == other.UId;
        }

        public override bool Equals(object obj)
        {
            return obj is PrimaryIndexComponentTestEntity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return UId.GetHashCode();
        }

        public readonly int FirstComponent
        {
            get => _domain.GetFirstComponent(this);
            set
            {
                var hasFirstComponent = HasFirstComponent;
                if (hasFirstComponent && !FirstComponent.Equals(value))
                {
                    _domain.ReplaceFirstComponent(this, value);
                }
                else if (!hasFirstComponent)
                {
                    _domain.AddFirstComponent(this, value);
                }
            }
        }

        public readonly bool HasFirstComponent => _domain.HasFirstComponent(this);
        public readonly void RemoveFirstComponent() => _domain.RemoveFirstComponent(this);
        public readonly int SecondComponent
        {
            get => _domain.GetSecondComponent(this);
            set
            {
                var hasSecondComponent = HasSecondComponent;
                if (hasSecondComponent && !SecondComponent.Equals(value))
                {
                    _domain.ReplaceSecondComponent(this, value);
                }
                else if (!hasSecondComponent)
                {
                    _domain.AddSecondComponent(this, value);
                }
            }
        }

        public readonly bool HasSecondComponent => _domain.HasSecondComponent(this);
        public readonly void RemoveSecondComponent() => _domain.RemoveSecondComponent(this);
    }
}