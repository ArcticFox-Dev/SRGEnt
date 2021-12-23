using System;

namespace SRGEnt.Aspects
{
    public readonly struct AspectIndex : IEquatable<AspectIndex>
    {
        private readonly byte _value;
        public readonly byte Element;
        public readonly byte Bit;

        public AspectIndex(byte value)
        {
            _value = value;
            Element = (byte) (_value / 8);
            Bit = (byte)(_value % 8);
        }

        public bool Equals(AspectIndex other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is AspectIndex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(AspectIndex left, AspectIndex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AspectIndex left, AspectIndex right)
        {
            return !left.Equals(right);
        }
        
        public static implicit operator byte(AspectIndex d) => d._value;
        public static implicit operator AspectIndex(byte b) => new AspectIndex(b);
    }
}
