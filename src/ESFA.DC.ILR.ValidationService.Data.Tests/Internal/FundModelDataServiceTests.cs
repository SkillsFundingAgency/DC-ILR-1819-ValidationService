using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class FundModelDataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            NewService().Exists(1).Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            NewService().Exists(2).Should().BeFalse();
        }

        private FundModelDataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.FundModels).Returns(new HashSet<int>() { 1 });

            return new FundModelDataService(internalDataCacheMock.Object);
        }
    }
}
