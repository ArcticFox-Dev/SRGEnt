using System.Collections.Generic;
using SRGEnt.Interfaces;

namespace SRGEnt.Groups.Utils
{
    public class EntityIndexComparer<T> : IComparer<T> where T : struct, IEntity
    {
        public int Compare(T x, T y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }
}