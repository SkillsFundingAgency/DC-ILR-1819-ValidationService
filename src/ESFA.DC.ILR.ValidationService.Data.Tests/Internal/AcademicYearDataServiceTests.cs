using System;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.Internal
{
    public class AcademicYearDataServiceTests
    {
        [Fact]
        public void AugustThirtyFirst()
        {
            var date = new DateTime(2017, 8, 31);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.AugustThirtyFirst).Returns(date);

            NewService(internalDataCacheMock.Object).AugustThirtyFirst().Should().Be(date);
        }

        [Fact]
        public void YearEnd()
        {
            var date = new DateTime(2018, 8, 31);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.End).Returns(date);

            NewService(internalDataCacheMock.Object).End().Should().Be(date);
        }

        [Fact]
        public void JanuaryFirst()
        {
            var date = new DateTime(2018, 1, 1);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.JanuaryFirst).Returns(date);

            NewService(internalDataCacheMock.Object).JanuaryFirst().Should().Be(date);
        }

        [Fact]
        public void YearStart()
        {
            var date = new DateTime(2017, 8, 1);

            var internalDataCacheMock = new Mock<IInternalDataCache>();

            internalDataCacheMock.SetupGet(c => c.AcademicYear.Start).Returns(date);

            NewService(internalDataCacheMock.Object).Start().Should().Be(date);
        }
        
        private AcademicYearDataService NewService(IInternalDataCache internalDataCache = null)
        {
            return new AcademicYearDataService(internalDataCache);
        }
    }
}
