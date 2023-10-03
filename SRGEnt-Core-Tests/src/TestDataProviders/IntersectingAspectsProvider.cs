using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders.Utilities;
using Xunit;

namespace SRGEnt.Tests.DataProviders
{
    public class IntersectingAspectsProvider : TheoryData<Aspect, Aspect>
    {
        public IntersectingAspectsProvider()
        {
            for (var i = 0; i < 10; i++)
            {
                var (aspectOne, aspectTwo) = AspectUtilities.GetTwoIntersectingAspects();
                Add(aspectOne,aspectTwo);
            }
        }
    }
}