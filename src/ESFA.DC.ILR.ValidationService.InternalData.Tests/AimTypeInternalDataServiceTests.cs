using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.InternalData.AimType;
using ESFA.DC.ILR.ValidationService.InternalData.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests
{
    public class AimTypeInternalDataServiceTests
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

        private AimTypeInternalDataService NewService()
        {
            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AimTypes).Returns(new HashSet<int>() { 1 });

            return new AimTypeInternalDataService(internalDataCacheMock.Object);
        }
    }
}
