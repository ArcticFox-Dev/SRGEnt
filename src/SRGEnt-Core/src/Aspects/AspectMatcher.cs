using System;

namespace SRGEnt.Aspects
{
    public abstract class AspectMatcher : IEquatable<AspectMatcher>
    {
        private int _hashCode;
        private bool CheckForRequired => !Required.Equals(Empty);
        private bool CheckForForbidden => !Forbidden.Equals(Empty);
        private bool CheckForAnyOf => !AnyOf.Equals(Empty);

        protected abstract ref readonly Aspect Empty { get; }
        
        protected readonly Aspect Required;
        protected readonly Aspect Forbidden;
        protected readonly Aspect AnyOf;
        
        protected AspectMatcher(int aspectSize)
        {
            AnyOf = new Aspect(aspectSize);
            Required = new Aspect(aspectSize);
            Forbidden = new Aspect(aspectSize);
        }

        public bool MatchesAspect(in Aspect aspect)
        {
            if (!CheckForRequired && !CheckForForbidden && !CheckForAnyOf) return false;

            var required = !CheckForRequired || aspect.ContainsAspect(Required);
            var forbidden = !CheckForForbidden || aspect.DoesNotContainAspect(Forbidden);
            var anyOf = !CheckForAnyOf || aspect.ContainsAnyPart(AnyOf);
            
            return required && forbidden && anyOf;
        }

        public bool Equals(AspectMatcher other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AspectMatcher) obj);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        protected void RecalculateHashCode()
        {
            _hashCode = Required.Size;
            _hashCode = unchecked(_hashCode * 32 + Required.GetHashCode());
            _hashCode = unchecked(_hashCode * 32 + Forbidden.GetHashCode());
            _hashCode = unchecked(_hashCode * 32 + AnyOf.GetHashCode());
        }
    }
}
