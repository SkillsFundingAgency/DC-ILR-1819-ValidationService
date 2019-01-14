using System;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class AcademicYearQueryServiceTests
    {
        [Theory]
        [InlineData(1, 2017, "2017-01-27")]
        [InlineData(2, 2017, "2017-02-24")]
        [InlineData(3, 2017, "2017-03-31")]
        [InlineData(4, 2017, "2017-04-28")]
        [InlineData(5, 2017, "2017-05-26")]
        [InlineData(12, 2016, "2016-12-30")]
        [InlineData(11, 2016, "2016-11-25")]
        [InlineData(6, 2016, "2016-06-24")]
        public void LastFridayInMonth(int month, int year, string expectedDate)
        {
            var date = new DateTime(year, month, 1);
            var expectedValue = DateTime.Parse(expectedDate);

            var academicYearQueryService = NewService();

            while (date.Month == month)
            {
                academicYearQueryService.LastFridayInMonth(date).Should().Be(expectedValue);
                date = date.AddDays(1);
            }
        }

        [Theory]
        [InlineData("2017-1-1", "2017-6-30")]
        [InlineData("2017-8-31", "2017-6-30")]
        [InlineData("2017-9-1", "2018-6-29")]
        public void LastFridayInJuneForDateInAcademicYear(string inputDate, string expectedDate)
        {
            var inputDateTime = DateTime.Parse(inputDate);
            var expectedDateTime = DateTime.Parse(expectedDate);

            NewService().LastFridayInJuneForDateInAcademicYear(inputDateTime).Should().Be(expectedDateTime);
        }

        [Fact]
        public void DateIsInPrevAcademicYear_True()
        {
            var date = new DateTime(2018, 7, 1);
            var academicYearStart = new DateTime(2018, 8, 1);

            NewService().DateIsInPrevAcademicYear(date, academicYearStart).Should().BeTrue();
        }

        [Fact]
        public void DateIsInPrevAcademicYear_False()
        {
            var date = new DateTime(2018, 9, 1);
            var academicYearStart = new DateTime(2018, 8, 1);

            NewService().DateIsInPrevAcademicYear(date, academicYearStart).Should().BeFalse();
        }

        private AcademicYearQueryService NewService()
        {
            return new AcademicYearQueryService();
        }
    }
}
