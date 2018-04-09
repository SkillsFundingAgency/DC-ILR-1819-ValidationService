using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.InternalData.CompStatus;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests
{
    public class CompStatusInternalDataServiceTests
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

        private CompStatusInternalDataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.CompStatuses).Returns(new HashSet<int>() { 1 });

            return new CompStatusInternalDataService(internalDataCacheMock.Object);
        }
    }
}
