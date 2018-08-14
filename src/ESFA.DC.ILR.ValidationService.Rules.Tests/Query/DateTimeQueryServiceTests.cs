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

        [Theory]
        [InlineData("2018-1-10", "2018-2-18", 1)]
        [InlineData("2018-1-10", "2018-3-10", 2)]
        [InlineData("2018-1-10", "2024-6-10", 77)]
        [InlineData("2018-1-10", "2018-1-12", 0)]
        [InlineData("2018-1-10", "2017-10-10", 3)]
        [InlineData("2018-1-10", "2020-1-10", 24)]
        public void MonthsBetween(string start, string end, int months)
        {
            new DateTimeQueryService().MonthsBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(months);
        }

        [Theory]
        [InlineData("2018-3-10", "2018-3-18", 8)]
        [InlineData("2018-3-10", "2018-3-10", 0)]
        [InlineData("2018-3-10", "2018-3-11", 1)]
        [InlineData("2018-3-10", "2018-3-29", 19)]
        [InlineData("2018-3-11", "2018-3-10", -1)]
        public void DaysBetween(string start, string end, double days)
        {
            new DateTimeQueryService().DaysBetween(DateTime.Parse(start), DateTime.Parse(end)).Should().Be(days);
        }

        [Theory]
        [InlineData("2002-04-12", 16, "2018-04-12")]
        [InlineData("2002-04-12", 0, "2002-04-12")]
        [InlineData("2002-04-12", -1, "2001-04-12")]
        public void DateAddYears(string date, int yearsToAdd, string newDate)
        {
            new DateTimeQueryService().DateAddYears(DateTime.Parse(date), yearsToAdd).Should().Be(DateTime.Parse(newDate));
        }
    }
}
