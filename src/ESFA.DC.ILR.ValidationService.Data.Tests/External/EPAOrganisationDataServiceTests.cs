using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class EPAOrganisationDataServiceTests
    {
        [Theory]
        [InlineData("EpaOrg1", 1, "2019-08-01")]
        [InlineData("EpaOrg1`", 1, "2020-09-01")]
        [InlineData("EpaOrg2", 2, "2018-08-01")]
        [InlineData("EpaOrg2", 3, "2018-08-01")]
        [InlineData("EpaOrg2", 3, "2020-08-01")]
        public void IsValidEpaOrg_True(string epaOrgId, int? stdCode, string learnPlanEndDate)
        {
            var epaOrganisations = new Dictionary<string, List<EPAOrganisations>>()
            {
                {
                    "EpaOrg1",
                    new List<EPAOrganisations>
                    {
                        new EPAOrganisations
                        {
                            ID = "EpaOrg1",
                            Standard = "1",
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    }
                },
                {
                    "EpaOrg2",
                    new List<EPAOrganisations>
                    {
                        new EPAOrganisations
                        {
                            ID = "EpaOrg2",
                            Standard = "2",
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        },
                        new EPAOrganisations
                        {
                            ID = "EpaOrg2",
                            Standard = "3",
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    }
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.EPAOrganisations).Returns(epaOrganisations);

            NewService(externalDataCacheMock.Object).IsValidEpaOrg("EpaOrg1", 1, new DateTime(2019, 6, 1)).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 1, "2019-08-01")]
        [InlineData("EpaOrg1", null, "2019-08-01")]
        [InlineData("EpaOrg1", 1, "2017-08-01")]
        [InlineData("EpaOrg2", 1, "2019-08-01")]
        [InlineData("EpaOrg1", 2, "2019-08-01")]
        public void IsValidEpaOrg_False(string epaOrgId, int? stdCode, string learnPlanEndDate)
        {
            var epaOrganisations = new Dictionary<string, List<EPAOrganisations>>()
            {
                {
                    "EpaOrg1",
                    new List<EPAOrganisations>
                    {
                        new EPAOrganisations
                        {
                            ID = "EpaOrg1",
                            Standard = "1",
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    }
                },
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.SetupGet(rdc => rdc.EPAOrganisations).Returns(epaOrganisations);

            NewService(externalDataCacheMock.Object).IsValidEpaOrg(epaOrgId, stdCode, DateTime.Parse(learnPlanEndDate)).Should().BeFalse();
        }

        private EPAOrganisationDataService NewService(IExternalDataCache externalDataCache)
        {
            return new EPAOrganisationDataService(externalDataCache);
        }
    }
}
