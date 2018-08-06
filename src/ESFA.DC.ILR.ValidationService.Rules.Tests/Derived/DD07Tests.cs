using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DD07Tests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        [InlineData(25)]
        public void IsApprenticeship_True(int? input)
        {
            NewDD().IsApprenticeship(input).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        [InlineData(null)]
        public void IsApprenticeship_False(int? input)
        {
            NewDD().IsApprenticeship(input).Should().BeFalse();
        }

        private DD07 NewDD()
        {
            return new DD07();
        }
    }
}
