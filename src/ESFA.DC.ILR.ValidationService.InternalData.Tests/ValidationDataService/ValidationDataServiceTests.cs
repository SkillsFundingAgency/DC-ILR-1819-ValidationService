using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.InternalData.Tests.ValidationDataService
{
    public class ValidationDataServiceTests
    {
        [Fact]
        public void AcademicYearAugustThirtyFirst()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.AcademicYearAugustThirtyFirst.Should().Be(new DateTime(2017, 8, 31));
        }

        [Fact]
        public void AcademicYearEnd()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.AcademicYearEnd.Should().Be(new DateTime(2018, 7, 31));
        }

        [Fact]
        public void AcademicYearJanuaryFirst()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.AcademicYearJanuaryFirst.Should().Be(new DateTime(2018, 1, 1));
        }

        [Fact]
        public void AcademicYearStart()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.AcademicYearStart.Should().Be(new DateTime(2017, 8, 1));
        }

        [Fact]
        public void ApprenticeshipProgTypes()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.ApprenticeProgTypes.Should().Equal(new HashSet<long>() { 2, 3, 20, 21, 2, 23, 25 });
        }

        [Fact]
        public void ApprenticeshipProgAllowedStartDate()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.ApprencticeProgAllowedStartDate.Should().Be(new DateTime(2016, 8, 1));
        }

        [Fact]
        public void ValidationStartDateTime()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            dateTimeProviderMock.SetupGet(dtp => dtp.UtcNow).Returns(new DateTime(1970, 1, 1));

            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(dateTimeProviderMock.Object);

            validationDataService.ValidationStartDateTime.Should().Be(new DateTime(1970, 1, 1));
        }

        [Fact]
        public void ValidationStartDateTime_Fallback()
        {
            var validationDataService = new InternalData.ValidationDataService.ValidationDataService(null);

            validationDataService.ValidationStartDateTime.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
