using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class PostcodeQueryServiceTests
    {
        [Theory]
        [InlineData("A1 1AA")]
        [InlineData("A11 1AA")]
        [InlineData("AA1 1AA")]
        [InlineData("AA11 1AA")]
        [InlineData("A1A 1AA")]
        [InlineData("AA1A 1AA")]
        public void RegexValid_True(string postcode)
        {
            NewService().RegexValid(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("AAA AAA")]
        [InlineData("1A 1AC")]
        [InlineData("1A 1AI")]
        [InlineData("1A 1AK")]
        [InlineData("1A 1AM")]
        [InlineData("1A 1AO")]
        [InlineData("1A 1AV")]
        public void RegexValid_False(string postcode)
        {
            NewService().RegexValid(postcode).Should().BeFalse();
        }

        private PostcodeQueryService NewService()
        {
            return new PostcodeQueryService();
        }
    }
}
