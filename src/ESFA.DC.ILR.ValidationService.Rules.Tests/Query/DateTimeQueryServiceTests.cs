using System;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class DateTimeQueryServiceTests
    {
        [Theory]
        [InlineData("1988-3-10", "2018-2-18", 29)]
        [InlineData("1988-3-10", "2018-3-10", 30)]
        [InlineData("1988-3-10", "1988-3-10", 0)]
        [InlineData("1988-3-10", "1989-3-10", 1)]
        [InlineData("1988-3-10", "1987-3-10", -1)]
        public void YearsBetween(string start, string end, int years)
        {
            new DateTimeQueryService().YearsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(years);
        }
    }
}
