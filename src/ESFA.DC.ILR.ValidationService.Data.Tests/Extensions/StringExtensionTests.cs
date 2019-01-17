using ESFA.DC.ILR.ValidationService.Data.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("Word", "Word")]
        [InlineData("Word", "WORD")]
        [InlineData("Word", "word")]
        public void CaseInsensitiveEquals_True(string lhs, string rhs)
        {
            lhs.CaseInsensitiveEquals(rhs).Should().BeTrue();
        }

        [Fact]
        public void CaseInsensitiveEquals_False()
        {
            "LHS".CaseInsensitiveEquals("RHS").Should().BeFalse();
        }

        [Fact]
        public void CaseInsensitiveEquals_Null_LHS()
        {
            string testString = null;

            testString.CaseInsensitiveEquals("RHS").Should().BeFalse();
        }

        [Fact]
        public void CaseInsensitiveEquals_Null_RHS()
        {
            string lhs = "LHS";

            lhs.CaseInsensitiveEquals(null).Should().BeFalse();
        }
    }
}
