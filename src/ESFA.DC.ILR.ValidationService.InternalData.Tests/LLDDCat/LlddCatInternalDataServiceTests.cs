using System.Linq;
using ESFA.DC.ILR.ValidationService.InternalData.LLDDCat;
using FluentAssertions;
using Xunit;
using DateTime = System.DateTime;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests.LLDDCat
{
    public class LlddCatInternalDataServiceTests
    {
        [Fact]
        public void CodeExists_True()
        {
            var llddCateService = new LlddCatInternalDataService();

            foreach (var num in Enumerable.Range(1, 17).Concat(Enumerable.Range(93, 7)))
            {
                llddCateService.CategoryExists(num).Should().BeTrue();
            }
        }

        [Fact]
        public void CodeExists_False()
        {
            var llddCateService = new LlddCatInternalDataService();

            foreach (var num in Enumerable.Range(18, 74).Concat(Enumerable.Range(100, 10)))
            {
                llddCateService.CategoryExists(num).Should().BeFalse();
            }
        }

        [Fact]
        public void CodeExists_Null_False()
        {
            var llddCateService = new LlddCatInternalDataService();
            llddCateService.CategoryExists(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, "2015-07-15")]
        [InlineData(2, "2015-07-15")]
        [InlineData(3, "2015-07-15")]
        [InlineData(4, "2099-12-31")]
        [InlineData(17, "2099-12-31")]
        [InlineData(93, "2099-12-31")]
        [InlineData(99, "2099-12-31")]
        public void TypeExistsForDate_True(long? code, string dateTime)
        {
            var llddCateService = new LlddCatInternalDataService();
            var validToDate = string.IsNullOrEmpty(dateTime) ? (DateTime?)null : DateTime.Parse(dateTime);
            llddCateService.CategoryExistForDate(code, validToDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "2015-07-15")]
        [InlineData(1, null)]
        [InlineData(18, "2015-07-15")]
        [InlineData(1, "2015-08-01")]
        [InlineData(100, "2099-12-31")]
        [InlineData(99, "2100-01-01")]
        public void TypeExistsForDate_False(long? code, string dateTime)
        {
            var llddCateService = new LlddCatInternalDataService();
            var validToDate = string.IsNullOrEmpty(dateTime) ? (DateTime?)null : DateTime.Parse(dateTime);
            llddCateService.CategoryExistForDate(code, validToDate).Should().BeFalse();
        }
    }
}