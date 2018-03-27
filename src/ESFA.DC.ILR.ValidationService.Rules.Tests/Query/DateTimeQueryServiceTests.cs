using System;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class DateTimeQueryServiceTests
    {
        [Theory]
        [InlineData("1988-2-10", "2018-1-18", 29)]
        [InlineData("1988-2-10", "2018-2-10", 30)]
        [InlineData("1988-2-10", "1988-2-10", 0)]
        [InlineData("1988-2-10", "1989-2-10", 1)]
        [InlineData("1988-2-10", "1987-2-10", -1)]
        public void YearsBetween(string start, string end, int years)
        {
            var queryService = new DateTimeQueryService();

            queryService.YearsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(years);
        }
    }
}
