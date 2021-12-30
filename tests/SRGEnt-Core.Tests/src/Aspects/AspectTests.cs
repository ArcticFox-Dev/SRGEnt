using FluentAssertions;
using SRGEnt.Aspects;
using SRGEnt.Tests.DataProviders;
using Xunit;

namespace SRGEnt.Tests.Aspects
{
    
    
    public class AspectTests
    {
        [Theory]
        [ClassData(typeof(DifferentAspectsProvider))]
        public void Two_Different_Aspects_Should_Return_Different_Hash_Codes(Aspect one, Aspect two)
        {
            one.GetHashCode().Should().NotBe(two.GetHashCode());
        }

        [Theory]
        [ClassData(typeof(IdenticalAspectsProvider))]
        public void Two_Identical_Aspects_Should_Return_Same_Hash_Codes(Aspect one, Aspect two)
        {
            one.GetHashCode().Should().Be(two.GetHashCode());
        }

        [Theory]
        [ClassData(typeof(IdenticalAspectsProvider))]
        public void When_Comparing_Two_Identical_Aspects_The_Comparison_Should_Return_True(Aspect one, Aspect two)
        {
            Aspect.AreAspectsEqual(one, two).Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(DifferentAspectsProvider))]
        public void When_Comparing_Two_Different_Aspects_The_Comparison_Should_Return_False(Aspect one, Aspect two)
        {
            Aspect.AreAspectsEqual(one, two).Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(DisjointedAspectsProvider))]
        public void When_Two_Aspects_With_No_Indices_In_Common_Are_Tested_For_Overlap_It_Should_Return_True(
            Aspect one, Aspect two)
        {
            one.DoesNotContainAnyPartOfAspect(two).Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(IntersectingAspectsProvider))]
        public void
            When_Two_Aspects_Have_At_Least_One_Index_In_Common_Testing_Them_For_Overlap_Should_Return_False(
                Aspect one, Aspect two)
        {
            one.DoesNotContainAnyPartOfAspect(two).Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(IntersectingAspectsProvider))]
        public void When_Two_Aspects_Have_At_Least_One_Index_In_Common_Testing_Them_For_Intersection_Should_Return_True(
            Aspect one, Aspect two)
        {
            one.ContainsAnyPart(two).Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(ContainedAspectProvider))]
        public void When_An_Aspect_Is_Checking_If_It_Contains_Its_SubAspect_The_Check_Should_Return_True(Aspect one,
            Aspect two)
        {
            one.ContainsAspect(two).Should().BeTrue();
        }
    }
}