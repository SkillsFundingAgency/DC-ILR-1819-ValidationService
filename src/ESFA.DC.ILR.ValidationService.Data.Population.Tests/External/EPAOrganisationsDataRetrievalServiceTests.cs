using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ReferenceData.EPA.Model;
using ESFA.DC.ReferenceData.EPA.Model.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class EPAOrganisationsDataRetrievalServiceTests
    {
        [Fact]
        public void UniqueUKPRNsFromMessage_NoLearners()
        {
            var message = new TestMessage();

            var epaOrganisationsDataRetrievalServiceMock = new Mock<EPAOrganisationsDataRetrievalService> { CallBase = true };

            epaOrganisationsDataRetrievalServiceMock.Setup(s => s.UniqueEpaOrgIdsFromMessage(message)).Returns(new List<string>());

            var epaOrganisations = epaOrganisationsDataRetrievalServiceMock.Object.UniqueEpaOrgIdsFromMessage(message).ToList();

            epaOrganisations.Should().BeEmpty();
            epaOrganisations.Should().HaveCount(0);
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_NoLearningDeliveries()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner()
                }
            };

            var epaOrganisationsDataRetrievalServiceMock = new Mock<EPAOrganisationsDataRetrievalService> { CallBase = true };

            epaOrganisationsDataRetrievalServiceMock.Setup(s => s.UniqueEpaOrgIdsFromMessage(message)).Returns(new List<string>());

            var epaOrganisations = epaOrganisationsDataRetrievalServiceMock.Object.UniqueEpaOrgIdsFromMessage(message).ToList();

            epaOrganisations.Should().BeEmpty();
            epaOrganisations.Should().HaveCount(0);
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_NoEPAOrgIds()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery()
                        }
                    }
                }
            };

            var epaOrganisationsDataRetrievalServiceMock = new Mock<EPAOrganisationsDataRetrievalService> { CallBase = true };

            epaOrganisationsDataRetrievalServiceMock.Setup(s => s.UniqueEpaOrgIdsFromMessage(message)).Returns(new List<string>());

            var epaOrganisations = epaOrganisationsDataRetrievalServiceMock.Object.UniqueEpaOrgIdsFromMessage(message).ToList();

            epaOrganisations.Should().BeEmpty();
            epaOrganisations.Should().HaveCount(0);
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_EPAOrgIds()
        {
            var epaOrgId = "EpaOrg001";

            var message = new TestMessage
            {
                Learners = new List<TestLearner>
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                EPAOrgID = epaOrgId
                            }
                        }
                    }
                }
            };

            var epaOrganisationsDataRetrievalServiceMock = new Mock<EPAOrganisationsDataRetrievalService> { CallBase = true };

            epaOrganisationsDataRetrievalServiceMock.Setup(s => s.UniqueEpaOrgIdsFromMessage(message)).Returns(new List<string> { epaOrgId });

            var epaOrganisations = epaOrganisationsDataRetrievalServiceMock.Object.UniqueEpaOrgIdsFromMessage(message).ToList();

            epaOrganisations.Should().HaveCount(1);
            epaOrganisations.Should().BeEquivalentTo(new List<string> { epaOrgId });
        }

        [Fact]
        public async Task Retrieve()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                EPAOrgID = "EpaOrg1"
                            }
                        }
                    },
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                 EPAOrgID = "EpaOrg2"
                            }
                        }
                    },
                }
            };

            var epaOrganisationsMock = new Mock<IEpaContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<Period> epaOrgPeriodList = new List<Period>
            {
                new Period
                {
                    OrganisationId = "EpaOrg1",
                    StandardCode = "1",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                },
                new Period
                {
                    OrganisationId = "EpaOrg2",
                    StandardCode = "2",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                },
                new Period
                {
                    OrganisationId = "EpaOrg2",
                    StandardCode = "22",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                }
            };

            var epaOrgPeriodsMock = epaOrgPeriodList.AsMockDbSet();

            epaOrganisationsMock.Setup(o => o.Periods).Returns(epaOrgPeriodsMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var epaOrgs = await NewService(epaOrganisationsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            epaOrgs.Select(k => k.Key).Should().HaveCount(2);
            epaOrgs.Select(k => k.Key).Should().Contain("EpaOrg1");
            epaOrgs.Select(k => k.Key).Should().Contain("EpaOrg2");

            epaOrgs.Where(k => k.Key == "EpaOrg1").Select(v => v.Value.Should().HaveCount(1));
            epaOrgs.Where(k => k.Key == "EpaOrg2").Select(v => v.Value.Should().HaveCount(2));
        }

        [Fact]
        public async Task Retrieve_EpaOrgID_MisMatch()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                EPAOrgID = "EpaOrg1"
                            }
                        }
                    },
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                 EPAOrgID = "EpaOrg2"
                            }
                        }
                    },
                }
            };

            var epaOrganisationsMock = new Mock<IEpaContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<Period> epaOrgPeriodList = new List<Period>
            {
                new Period
                {
                    OrganisationId = "EpaOrg1",
                    StandardCode = "1",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                },
                new Period
                {
                    OrganisationId = "EpaOrg3",
                    StandardCode = "2",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                },
                new Period
                {
                    OrganisationId = "EpaOrg3",
                    StandardCode = "22",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                }
            };

            var epaOrgPeriodsMock = epaOrgPeriodList.AsMockDbSet();

            epaOrganisationsMock.Setup(o => o.Periods).Returns(epaOrgPeriodsMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var epaOrgs = await NewService(epaOrganisationsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            epaOrgs.Select(k => k.Key).Should().HaveCount(1);
            epaOrgs.Select(k => k.Key).Should().Contain("EpaOrg1");
            epaOrgs.Select(k => k.Key).Should().NotContain("EpaOrg2");

            epaOrgs.Where(k => k.Key == "EpaOrg1").Select(v => v.Value.Should().HaveCount(1));
            epaOrgs.Where(k => k.Key == "EpaOrg2").Select(v => v.Value).Should().BeNullOrEmpty();
        }

        private EPAOrganisationsDataRetrievalService NewService(IEpaContext epaOrganisations = null, ICache<IMessage> messageCache = null)
        {
            return new EPAOrganisationsDataRetrievalService(epaOrganisations, messageCache);
        }
    }
}
