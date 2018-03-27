using ESFA.DC.ILR.ValidationService.InternalData.PriorAttain;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests.PriorAttain
{
    public class PriorAttainReferenceDataServiceTests
    {
        [Theory]
        [InlineData(new long[] { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 })]
        public void Exists_True(long[] attainValues)
        {
            var priorAttainReferenceDataService = new PriorAttainInternalDataService();

            foreach (var value in attainValues)
            {
                priorAttainReferenceDataService.Exists(value).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(new long[] { 22, 90, 100 })]
        public void Exists_False(long[] attainValues)
        {
            var priorAttainReferenceDataService = new PriorAttainInternalDataService();

            foreach (var value in attainValues)
            {
                priorAttainReferenceDataService.Exists(value).Should().BeFalse();
            }
        }
    }
}