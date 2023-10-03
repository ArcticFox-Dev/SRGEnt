using System;
using System.Linq;
using SRGEnt.Aspects;

namespace SRGEnt.Tests.DataProviders.Utilities
{
    public static class AspectUtilities
    {
        private static readonly Random _rng = new Random(DateTime.Now.Millisecond);
        
        private readonly struct AspectConstructionParameters
        {
            public readonly int AspectSize;
            public readonly int ElementsCount;
            public readonly int SetElements;

            public AspectConstructionParameters(int aspectSize, int elementsCount, int setElements)
            {
                AspectSize = aspectSize;
                ElementsCount = elementsCount;
                SetElements = setElements;
            }
        }

        

        public static (Aspect one, Aspect two) GetTwoIdenticalAspects()
        {
            var parameters = GetAspectBaseParameters();

            var index = _rng.Next(0, parameters.ElementsCount - parameters.SetElements - 1);

            return GetAspects(parameters, index, index);
        }

        

        public static (Aspect one, Aspect two) GetTwoDifferentAspects()
        {
            var parameters = GetAspectBaseParameters();

            var aspectOneStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);
            var aspectTwoStartIndex = 0;
            do
            {
                aspectTwoStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);
            } while (aspectOneStartIndex == aspectTwoStartIndex);

            return GetAspects(parameters, aspectOneStartIndex, aspectTwoStartIndex);
        }
        
        public static (Aspect one, Aspect two) GetTwoDisjointedAspects()
        {
            var parameters = GetAspectBaseParameters();
            parameters = new(parameters.AspectSize, parameters.ElementsCount, parameters.SetElements / 2);
            

            var aspectOneStartIndex = 0;
            var aspectTwoStartIndex = 0;
            do
            {
                aspectOneStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);
                aspectTwoStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);
            } while (Math.Abs(aspectOneStartIndex - aspectTwoStartIndex) <= parameters.SetElements);

            return GetAspects(parameters, aspectOneStartIndex, aspectTwoStartIndex);
        }
        
        public static (Aspect one, Aspect two) GetTwoIntersectingAspects()
        {
            var parameters = GetAspectBaseParameters();
            parameters = new(parameters.AspectSize, parameters.ElementsCount, Math.Max(2, parameters.SetElements / 2));
            var intersectionRange = _rng.Next(parameters.SetElements - 1);
            
            var aspectOneStartIndex = 0;
            var aspectTwoStartIndex = 0;
            do
            {
                aspectOneStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);
                aspectTwoStartIndex = aspectOneStartIndex - intersectionRange;
            } while (aspectTwoStartIndex < 0);

            return GetAspects(parameters, aspectOneStartIndex, aspectTwoStartIndex);
        }

        public static (Aspect one, Aspect two) GetAnAspectAndItsSubAspect()
        {
            var parameters = GetAspectBaseParameters();
            parameters = new(parameters.AspectSize, parameters.ElementsCount, Math.Max(2, parameters.SetElements / 2));
            var intersectionRange = _rng.Next(parameters.SetElements - 1);
            
            var aspectOneStartIndex = _rng.Next(0, parameters.ElementsCount - parameters.SetElements);;

            return GetAspectsWithSecondBeingSubAspect(parameters, aspectOneStartIndex);
        }
        
        private static AspectConstructionParameters GetAspectBaseParameters()
        {
            var aspectSize = _rng.Next(AspectTestsConstants.MinAspectSize, AspectTestsConstants.MaxAspectSize);
            var aspectElementsCount = aspectSize * AspectTestsConstants.IndicesPerAspectSize;
            var setElementsCount = _rng.Next(1, aspectSize * AspectTestsConstants.MaxElementsPerAspectSize);
            return new (aspectSize, aspectElementsCount, setElementsCount);
        }
        
        private static Span<byte> GenerateAllPossibleElements(int elementCount, bool randomize = true)
        {
            var elements = ParallelEnumerable.Range(0, elementCount).Select(x => (byte)x).ToArray();
            if (randomize)
            {
                _rng.Shuffle(elements);
            }
            return new Span<byte>(elements);
        }

        private static (Aspect one, Aspect two) GetAspects(AspectConstructionParameters parameters,
            int aspectOneStartIndex, int aspectTwoStartIndex)
        {
            var allPossibleElements = GenerateAllPossibleElements(parameters.ElementsCount);
            var aspectOneElements = allPossibleElements.Slice(aspectOneStartIndex, parameters.SetElements);
            var aspectTwoElements = allPossibleElements.Slice(aspectTwoStartIndex, parameters.SetElements);

            var aspectOne = new Aspect(parameters.AspectSize, aspectOneElements.ToArray());
            var aspectTwo = new Aspect(parameters.AspectSize, aspectTwoElements.ToArray());
            return (aspectOne, aspectTwo);
        }

        private static (Aspect one, Aspect two) GetAspectsWithSecondBeingSubAspect(AspectConstructionParameters parameters,
            int index)
        {
            var allPossibleElements = GenerateAllPossibleElements(parameters.ElementsCount);
            var aspectOneElements = allPossibleElements.Slice(index, parameters.SetElements);
            var aspectTwoElements = allPossibleElements.Slice(index + 1, parameters.SetElements / 2);

            var aspectOne = new Aspect(parameters.AspectSize, aspectOneElements.ToArray());
            var aspectTwo = new Aspect(parameters.AspectSize, aspectTwoElements.ToArray());
            return (aspectOne, aspectTwo);
        }
    }
}