using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Rules.Extensions;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Extensions
{
    public class LongExtensionTests
    {
        [Fact]
        public void ToDigitEnumerable()
        {
            ((long)1234).ToDigitEnumerable().Should().Equal(new List<int>() { 1, 2, 3, 4 });
            ((long)999999999).ToDigitEnumerable().Should().Equal(new List<int>() { 9, 9, 9, 9, 9, 9, 9, 9, 9 });
            ((long)0).ToDigitEnumerable().Should().Equal(new List<int>() { 0 });
            ((long)777).ToDigitEnumerable().Should().Equal(new List<int>() { 7, 7, 7 });
            ((long)1).ToDigitEnumerable().Should().Equal(new List<int>() { 1 });
        }
    }
}
