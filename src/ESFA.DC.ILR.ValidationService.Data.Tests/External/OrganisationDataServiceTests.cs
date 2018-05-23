﻿using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class OrganisationDataServiceTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void UkprnExists_True(long ukprn)
        {
            var organisationDictionary = new Dictionary<long, Organisation>()
            {
                { 1, null },
                { 2, null },
                { 3, null },
                { 4, null },
                { 5, null },
                { 6, null },
                { 7, null },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.Organisations).Returns(organisationDictionary);

            NewService(externalDataCacheMock.Object).UkprnExists(ukprn).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(8)]
        public void UkprnExists_False(long ukprn)
        {
            var organisationDictionary = new Dictionary<long, Organisation>()
            {
                { 1, null },
                { 2, null },
                { 3, null },
                { 4, null },
                { 5, null },
                { 6, null },
                { 7, null },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.Organisations).Returns(organisationDictionary);

            NewService(externalDataCacheMock.Object).UkprnExists(ukprn).Should().BeFalse();
        }

        [Fact]
        public void LegalOrgTypeMatchForUKPRN_True()
        {
            var ukprn = 1;
            var legalOrgType = "A";

            var organisationDictionary = new Dictionary<long, Organisation>()
            {
                { ukprn, new Organisation() { UKPRN = ukprn, LegalOrgType = legalOrgType } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.Organisations).Returns(organisationDictionary);

            NewService(externalDataCacheMock.Object).LegalOrgTypeMatchForUkprn(ukprn, legalOrgType).Should().BeTrue();
        }

        [Fact]
        public void LegalOrgTypeMatchForUKPRN_False_Null()
        {
            var ukprn = 1;
            var legalOrgType = "A";

            var organisationDictionary = new Dictionary<long, Organisation>()
            {
                { ukprn, new Organisation() { UKPRN = ukprn, LegalOrgType = legalOrgType } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.Organisations).Returns(organisationDictionary);

            NewService(externalDataCacheMock.Object).LegalOrgTypeMatchForUkprn(2, legalOrgType).Should().BeFalse();
        }

        [Fact]
        public void LegalOrgTypeMatchForUKPRN_False_Mismatch()
        {
            var ukprn = 1;
            var legalOrgType = "A";

            var organisationDictionary = new Dictionary<long, Organisation>()
            {
                { ukprn, new Organisation() { UKPRN = ukprn, LegalOrgType = legalOrgType } },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.Organisations).Returns(organisationDictionary);

            NewService(externalDataCacheMock.Object).LegalOrgTypeMatchForUkprn(ukprn, "B").Should().BeFalse();
        }

        private OrganisationDataService NewService(IExternalDataCache externalDataCache)
        {
            return new OrganisationDataService(externalDataCache);
        }
    }
}
