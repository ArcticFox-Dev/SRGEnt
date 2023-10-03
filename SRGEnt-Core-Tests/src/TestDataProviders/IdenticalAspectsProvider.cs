using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders.Utilities;
using Xunit;

namespace SRGEnt.Tests.DataProviders
{
    public class IdenticalAspectsProvider : TheoryData<Aspect, Aspect>
    {
        public IdenticalAspectsProvider()
        {
            for (var i = 0; i < 10; i++)
            {
                var (aspectOne, aspectTwo) = AspectUtilities.GetTwoIdenticalAspects();
                Add(aspectOne,aspectTwo);
            }
        }
    }
}