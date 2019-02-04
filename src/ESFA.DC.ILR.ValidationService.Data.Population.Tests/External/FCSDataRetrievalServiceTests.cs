using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class FCSDataRetrievalServiceTests
    {
        [Fact]
        public void UKPRNFromMessage()
        {
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 12345678
                }
            };

            var result = NewService().UKPRNFromMessage(message);

            result.Should().Be(12345678);
        }

        /// <summary>
        /// Retrieve Contract Allocations Async.
        /// </summary>
        /// <returns>the test task</returns>
        [Fact]
        public async Task RetrieveAsync()
        {
            // arrange
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new List<TestLearner>
                {
                    new TestLearner
                    {
                        LearningDeliveries = new List<TestLearningDelivery>
                        {
                            new TestLearningDelivery
                            {
                                ConRefNumber = "100"
                            },
                            new TestLearningDelivery
                            {
                                ConRefNumber = "101"
                            },
                            new TestLearningDelivery
                            {
                                ConRefNumber = "103"
                            },
                            new TestLearningDelivery
                            {
                                ConRefNumber = "104"
                            }
                        }
                    }
                }
            };

            var allocations = new List<ContractAllocation>
            {
                new ContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1,
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "103",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "104",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "105",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 2
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "106",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 2
                }
            }.AsMockDbSet();

            var eligibilityRules = new List<EsfEligibilityRule>()
            {
                new EsfEligibilityRule()
                {
                    EsfEligibilityRuleEmploymentStatuses = new List<EsfEligibilityRuleEmploymentStatus>(),
                    EsfEligibilityRuleLocalAuthorities = new List<EsfEligibilityRuleLocalAuthority>(),
                    EsfEligibilityRuleLocalEnterprisePartnerships = new List<EsfEligibilityRuleLocalEnterprisePartnership>(),
                    EsfEligibilityRuleSectorSubjectAreaLevel = new List<EsfEligibilityRuleSectorSubjectAreaLevel>(),
                }
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);

            fcsMock.Setup(f => f.EsfEligibilityRules).Returns(eligibilityRules);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var service = NewService(fcsMock.Object, messageCacheMock.Object);

            // act
            var fcsa = await service.RetrieveAsync(CancellationToken.None);

            // assert
            Assert.Equal(4, fcsa.Count);
        }

        [Fact]
        public void ConRefNumbersFromMessage()
        {
            var message = new TestMessage()
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new TestLearningDelivery[]
                        {
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123456",
                                ConRefNumber = "ESF-123"
                            }
                        }
                    }
                }
            };

            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = this.NewService(fcsMock.Object, messageCacheMock.Object).ConRefNumbersFromMessage(message);
            fcsa.Should().HaveCount(1);
        }

        [Fact]
        public void ConRefNumbersFromMessage_NoMessage()
        {
            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            var fcsa = this.NewService(fcsMock.Object, messageCacheMock.Object).ConRefNumbersFromMessage(null);
            fcsa.Should().BeNull();
        }

        [Fact]
        public void ConRefNumbersFromMessage_NoLearners()
        {
            var message = new TestMessage()
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                }
            };
            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
               .SetupGet(mc => mc.Item)
               .Returns(message);

            var fcsa = this.NewService(fcsMock.Object, messageCacheMock.Object).ConRefNumbersFromMessage(message);
            fcsa.Should().BeNull();
        }

        [Fact]
        public void ConRefNumbersFromMessage_NoLearnerDeliveries()
        {
            var message = new TestMessage()
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                    }
                }
            };
            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
               .SetupGet(mc => mc.Item)
               .Returns(message);

            var fcsa = this.NewService(fcsMock.Object, messageCacheMock.Object).ConRefNumbersFromMessage(message);
            fcsa.Should().BeEmpty();
        }

        [Fact]
        public void ConRefNumbersFromMessage_NoConRefNumbers()
        {
            var message = new TestMessage()
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                },
                Learners = new TestLearner[]
                {
                    new TestLearner()
                    {
                        LearningDeliveries = new TestLearningDelivery[]
                        {
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123456"
                            }
                        }
                    }
                }
            };
            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
               .SetupGet(mc => mc.Item)
               .Returns(message);

            var fcsa = this.NewService(fcsMock.Object, messageCacheMock.Object).ConRefNumbersFromMessage(message);
            fcsa.Should().BeEmpty();
        }

        private FCSDataRetrievalService NewService(IFcsContext fcs = null, ICache<IMessage> messageCache = null)
        {
            return new FCSDataRetrievalService(fcs, messageCache);
        }
    }
}
