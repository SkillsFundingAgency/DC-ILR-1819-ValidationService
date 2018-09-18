using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Organisatons.Model;
using ESFA.DC.Data.Organisatons.Model.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class OrganisationsDataRetrievalServiceTests
    {
        [Fact]
        public void UniqueUKPRNsFromMessage_NoLearningProvider()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>());

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().BeEmpty();
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_NoLearners()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPrevUKPRNsFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPMUKPRNsFromMessage(message)).Returns(new List<long>());

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().HaveCount(0);
            organisations.Should().BeEmpty();
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_NoLearningDeliveries()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningDeliveryPartnerUKPRNsFromMessage(message)).Returns(new List<long>());

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().HaveCount(0);
            organisations.Should().BeEmpty();
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_NoUKPRNs()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPrevUKPRNsFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPMUKPRNsFromMessage(message)).Returns(new List<long>());
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningDeliveryPartnerUKPRNsFromMessage(message)).Returns(new List<long>());

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().HaveCount(0);
            organisations.Should().BeEmpty();
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_UKPRNs()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>() { 1 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPrevUKPRNsFromMessage(message)).Returns(new List<long>() { 2 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPMUKPRNsFromMessage(message)).Returns(new List<long>() { 3 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningDeliveryPartnerUKPRNsFromMessage(message)).Returns(new List<long>() { 4 });

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().HaveCount(4);
            organisations.Should().Contain(1);
            organisations.Should().Contain(2);
            organisations.Should().Contain(3);
            organisations.Should().Contain(4);
        }

        [Fact]
        public void UniqueUKPRNsFromMessage_DistinctUKPRNs()
        {
            var message = new TestMessage();

            var organisationsDataRetrievalServiceMock = new Mock<OrganisationsDataRetrievalService> { CallBase = true };

            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningProviderUKPRNFromMessage(message)).Returns(new List<long>() { 1 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPrevUKPRNsFromMessage(message)).Returns(new List<long>() { 1 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearnerPMUKPRNsFromMessage(message)).Returns(new List<long>() { 3 });
            organisationsDataRetrievalServiceMock.Setup(s => s.UniqueLearningDeliveryPartnerUKPRNsFromMessage(message)).Returns(new List<long>() { 4 });

            var organisations = organisationsDataRetrievalServiceMock.Object.UniqueUKPRNsFromMessage(message).ToList();

            organisations.Should().HaveCount(3);
            organisations.Should().Contain(1);
            organisations.Should().Contain(3);
            organisations.Should().Contain(4);
        }

        [Fact]
        public void UniqueLearningProviderUKPRNFromMessage()
        {
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                }
            };

            var organisations = NewService().UniqueLearningProviderUKPRNFromMessage(message);

            organisations.Should().HaveCount(1);
            organisations.Should().Contain(1);
        }

        [Fact]
        public void UniqueLearnerPrevUKPRNsFromMessage()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PrevUKPRNNullable = 1
                    },
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2
                    },
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPrevUKPRNsFromMessage(message);

            organisations.Should().HaveCount(2);
            organisations.Should().Contain(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public void UniqueLearnerPrevUKPRNsFromMessage_Null()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPrevUKPRNsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnerPrevUKPRNsFromMessage_MixedNull()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PrevUKPRNNullable = null
                    },
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2
                    },
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPrevUKPRNsFromMessage(message);

            organisations.Should().HaveCount(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public void UniqueLearnerPMUKPRNsFromMessage()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PMUKPRNNullable = 1
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 2
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 2
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPMUKPRNsFromMessage(message);

            organisations.Should().HaveCount(2);
            organisations.Should().Contain(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public void UniqueLearnerPMUKPRNsFromMessage_Null()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPMUKPRNsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearnerPMUKPRNsFromMessage_MixedNull()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PMUKPRNNullable = null
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 2
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 2
                    },
                }
            };

            var organisations = NewService().UniqueLearnerPMUKPRNsFromMessage(message);

            organisations.Should().HaveCount(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public void UniqueLearningDeliveryPartnerUKPRNsFromMessage()
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
                                PartnerUKPRNNullable = 1
                            }
                        }
                    },
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 2
                            }
                        }
                    },
                }
            };

            var organisations = NewService().UniqueLearningDeliveryPartnerUKPRNsFromMessage(message);

            organisations.Should().HaveCount(2);
            organisations.Should().Contain(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public void UniqueLearningDeliveryPartnerUKPRNsFromMessage_Null()
        {
            var message = new TestMessage
            {
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                    },
                }
            };

            var organisations = NewService().UniqueLearningDeliveryPartnerUKPRNsFromMessage(message).Should().BeEmpty();
        }

        [Fact]
        public void UniqueLearningDeliveryPartnerUKPRNsFromMessage_MixedNull()
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
                                PartnerUKPRNNullable = null
                            }
                        }
                    },
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 2
                            }
                        }
                    },
                }
            };

            var organisations = NewService().UniqueLearningDeliveryPartnerUKPRNsFromMessage(message);

            organisations.Should().HaveCount(1);
            organisations.Should().Contain(2);
        }

        [Fact]
        public async Task Retrieve()
        {
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2,
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 22
                            }
                        }
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 3,
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 33
                            }
                        }
                    },
                }
            };

            var organisationsMock = new Mock<IOrganisations>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<MasterOrganisation> masterOrgList = new List<MasterOrganisation>
            {
                new MasterOrganisation
                {
                    UKPRN = 1,
                    Org_Details = new Org_Details
                    {
                        UKPRN = 1,
                        LegalOrgType = "LegalType1"
                    },
                   Org_PartnerUKPRN = new List<Org_PartnerUKPRN>
                   {
                       new Org_PartnerUKPRN
                       {
                           UKPRN = 1,
                           NameLegal = "NameLegal1"
                       }
                   }
                },
                new MasterOrganisation
                {
                    UKPRN = 2,
                    Org_Details = new Org_Details
                    {
                        UKPRN = 2,
                        LegalOrgType = "LegalType2"
                    }
                },
            };

            var masterOrgMock = masterOrgList.AsMockDbSet();

            organisationsMock.Setup(o => o.MasterOrganisations).Returns(masterOrgMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var organisations = await NewService(organisationsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            organisations.Select(k => k.Key).Should().HaveCount(2);
            organisations.Select(k => k.Key).Should().Contain(1);
            organisations.Select(k => k.Key).Should().Contain(2);

            organisations.Where(k => k.Key == 1).Select(v => v.Value.UKPRN).Single().Should().Be(1);
            organisations.Where(k => k.Key == 1).Select(v => v.Value.LegalOrgType).Single().Should().Be("LegalType1");
            organisations.Where(k => k.Key == 1).Select(v => v.Value.PartnerUKPRN).Single().Should().BeTrue();

            organisations.Where(k => k.Key == 2).Select(v => v.Value.UKPRN).Single().Should().Be(2);
            organisations.Where(k => k.Key == 2).Select(v => v.Value.LegalOrgType).Single().Should().Be("LegalType2");
            organisations.Where(k => k.Key == 2).Select(v => v.Value.PartnerUKPRN).Single().Should().BeFalse();
        }

        [Fact]
        public async Task Retrieve_UKPRN_Mismatch()
        {
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new List<TestLearner>()
                {
                    new TestLearner
                    {
                        PrevUKPRNNullable = 2,
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 22
                            }
                        }
                    },
                    new TestLearner
                    {
                        PMUKPRNNullable = 3,
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                PartnerUKPRNNullable = 33
                            }
                        }
                    },
                }
            };

            var organisationsMock = new Mock<IOrganisations>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            IEnumerable<MasterOrganisation> masterOrgList = new List<MasterOrganisation>
            {
                new MasterOrganisation
                {
                    UKPRN = 1,
                    Org_Details = new Org_Details
                    {
                        UKPRN = 1,
                        LegalOrgType = "LegalType1"
                    },
                    Org_PartnerUKPRN = new List<Org_PartnerUKPRN>
                    {
                        new Org_PartnerUKPRN
                        {
                            UKPRN = 1,
                            NameLegal = "NameLegal1"
                        }
                    }
                },
                new MasterOrganisation
                {
                    UKPRN = 2,
                    Org_Details = new Org_Details
                    {
                        UKPRN = 2,
                        LegalOrgType = "LegalType2"
                    }
                },
                 new MasterOrganisation
                {
                    UKPRN = 3
                },
            };

            var masterOrgMock = IEnumerableExtensions.AsMockDbSet(masterOrgList);

            organisationsMock.Setup(o => o.MasterOrganisations).Returns(masterOrgMock);

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var organisations = await NewService(organisationsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            organisations.Select(k => k.Key).Should().HaveCount(3);
            organisations.Select(k => k.Key).Should().Contain(new List<long> { 1, 2, 3 });

            organisations.Where(k => k.Key == 1).Select(v => v.Value.UKPRN).Single().Should().Be(1);
            organisations.Where(k => k.Key == 1).Select(v => v.Value.LegalOrgType).Single().Should().Be("LegalType1");
            organisations.Where(k => k.Key == 1).Select(v => v.Value.PartnerUKPRN).Single().Should().BeTrue();

            organisations.Where(k => k.Key == 2).Select(v => v.Value.UKPRN).Single().Should().Be(2);
            organisations.Where(k => k.Key == 2).Select(v => v.Value.LegalOrgType).Single().Should().Be("LegalType2");
            organisations.Where(k => k.Key == 2).Select(v => v.Value.PartnerUKPRN).Single().Should().BeFalse();

            organisations.Where(k => k.Key == 3).Select(v => v.Value.UKPRN).Single().Should().BeNull();
            organisations.Where(k => k.Key == 3).Select(v => v.Value.LegalOrgType).Single().Should().BeNull();
            organisations.Where(k => k.Key == 3).Select(v => v.Value.PartnerUKPRN).Single().Should().BeFalse();
        }

        private OrganisationsDataRetrievalService NewService(IOrganisations organisations = null, ICache<IMessage> messageCache = null)
        {
            return new OrganisationsDataRetrievalService(organisations, messageCache);
        }
    }
}
