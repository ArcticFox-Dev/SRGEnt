using System;
using SRGEnt.Enums;

namespace SRGEnt.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexComponentAttribute : Attribute
    {
        private readonly IndexType _indexType;
        public IndexComponentAttribute(IndexType indexType = IndexType.Primary)
        {
            _indexType = indexType;
        }
    }
}