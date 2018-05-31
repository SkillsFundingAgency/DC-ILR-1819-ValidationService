using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class CompStatusDataServiceTests
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

        private CompStatusDataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.CompStatuses).Returns(new HashSet<int>() { 1 });

            return new CompStatusDataService(internalDataCacheMock.Object);
        }
    }
}
