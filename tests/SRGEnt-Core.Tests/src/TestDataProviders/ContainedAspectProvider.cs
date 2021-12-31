using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders.Utilities;
using Xunit;

namespace SRGEnt.Tests.DataProviders
{
    public class ContainedAspectProvider : TheoryData<Aspect, Aspect>
    {
        public ContainedAspectProvider()
        {
            for (var i = 0; i < 10; i++)
            {
                var (aspectOne, aspectTwo) = AspectUtilities.GetAnAspectAndItsSubAspect();
                Add(aspectOne,aspectTwo);
            }
        }
    }
}