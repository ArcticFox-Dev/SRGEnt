using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders.Utilities;
using Xunit;

namespace SRGEnt.Tests.DataProviders
{
    public class DifferentAspectsProvider : TheoryData<Aspect, Aspect>
    {
        public DifferentAspectsProvider()
        {
            for (var i = 0; i < 10; i++)
            {
                var (aspectOne, aspectTwo) = AspectUtilities.GetTwoDifferentAspects();
                Add(aspectOne,aspectTwo);
            }
        }
    }
}