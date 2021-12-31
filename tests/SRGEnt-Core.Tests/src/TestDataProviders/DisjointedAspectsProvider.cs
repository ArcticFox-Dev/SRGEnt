using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders.Utilities;
using Xunit;

namespace SRGEnt.Tests.DataProviders
{
    public class DisjointedAspectsProvider : TheoryData<Aspect, Aspect>
    {
        public DisjointedAspectsProvider()
        {
            for (var i = 0; i < 10; i++)
            {
                var (aspectOne, aspectTwo) = AspectUtilities.GetTwoDisjointedAspects();
                Add(aspectOne,aspectTwo);
            }
        }
    }
}