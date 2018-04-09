using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.ULN;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class ULNReferenceDataServiceTests
    {
        [Fact]
        public void Exists_True()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ULNs).Returns(new HashSet<long>() { 1, 2, 3 });

            var ulnReferenceDataService = new ULNReferenceDataService(referenceDataCacheMock.Object);

            ulnReferenceDataService.Exists(2).Should().BeTrue();
        }

        [Fact]
        public void Exists_False()
        {
            var referenceDataCacheMock = new Mock<IExternalDataCache>();

            referenceDataCacheMock.SetupGet(rdc => rdc.ULNs).Returns(new HashSet<long>() { 1, 2, 3 });

            var ulnReferenceDataService = new ULNReferenceDataService(referenceDataCacheMock.Object);

            ulnReferenceDataService.Exists(4).Should().BeFalse();
        }

        [Fact]
        public void Exists_False_Null()
        {
            var ulnReferenceDataService = new ULNReferenceDataService(null);

            ulnReferenceDataService.Exists(null).Should().BeFalse();
        }
    }
}
