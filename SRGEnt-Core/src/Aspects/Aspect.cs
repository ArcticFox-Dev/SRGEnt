using System;

namespace SRGEnt.Aspects
{
    public class Aspect : IEquatable<Aspect>
    {
        public int Size => _aspect.Length;
        private readonly byte[] _aspect;
        private int _hashCode = 0;
        
        public Aspect(int aspectSize) => _aspect = new byte[aspectSize];

        public Aspect(int aspectSize, params byte[] componentBits)
        {
            _aspect = new byte[aspectSize];
            _hashCode = Size;
            foreach (var bit in componentBits)
            {
                var aspectIndex = new AspectIndex(bit);
                SetIndexQuietly(aspectIndex);
            }
            RecalculateHashCode();
        }

        public Aspect(Aspect aspect)
        {
            _aspect = new byte[aspect._aspect.Length];
            aspect._aspect.CopyTo(_aspect, 0);
        }

        public void Set(AspectIndex index)
        {
            SetIndexQuietly(index);
            RecalculateHashCode();
        }

        public void Set(params AspectIndex[] indices)
        {
            foreach (var index in indices)
            {
                SetIndexQuietly(index);
            }
            RecalculateHashCode();
        }

        public void Remove(AspectIndex index) => _aspect[index.Element] &= (byte) (byte.MaxValue ^ (byte) (1 << index.Bit));

        public void Remove(params AspectIndex[] indices)
        {
            foreach (var index in indices)
            {
                Remove(index);
            }
        }

        public bool ContainsComponent(AspectIndex index) => 0 != (_aspect[index.Element] & (byte) (1 << index.Bit));

        public bool ContainsAspect(Aspect b)
        {
            if (_aspect.Length != b._aspect.Length) return false;

            var length = _aspect.Length;
            for (int i = 0; i < length; i++)
            {
                if ((_aspect[i] & b._aspect[i]) != b._aspect[i]) return false;
            }
            return true;
        }

        public bool ContainsAnyPart(Aspect b)
        {
            if (_aspect.Length != b._aspect.Length) return false;

            var length = _aspect.Length;
            for (int i = 0; i < length; i++)
            {
                if ((_aspect[i] & b._aspect[i]) > 0) return true;
            }
            return false;
        }

        public bool DoesNotContainAnyPartOfAspect(Aspect b)
        {
            if (_aspect.Length != b._aspect.Length) return false;

            var length = _aspect.Length;
            for (int i = 0; i < length; i++)
            {
                if ((_aspect[i] & b._aspect[i]) != 0) return false;
            }
            return true;
        }

        public static bool AreAspectsEqual(in Aspect a, in Aspect b)
        {
            if (a._aspect.Length != b._aspect.Length) return false;

            var length = a._aspect.Length;
            var equal = true;
            for (int i = 0; i < length && equal; i++)
            {
                equal = a._aspect[i] == b._aspect[i];
            }

            return equal;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
        public bool Equals(Aspect other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Aspect) obj);
        }
        
        private void SetIndexQuietly(AspectIndex index) => _aspect[index.Element] |= (byte) (1 << index.Bit);
        
        private void RecalculateHashCode()
        {
            _hashCode = Size;
            for (var i = 0; i < Size; i++)
                _hashCode = unchecked(_hashCode * 257 + _aspect[i]);
        }

    }
}
