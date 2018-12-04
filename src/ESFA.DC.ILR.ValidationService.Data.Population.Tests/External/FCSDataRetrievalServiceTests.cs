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

        [Fact]
        public async Task RetrieveAsync()
        {
            // arrange
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                }
            };

            var fcsContractor = new List<Contractor>
            {
                new Contractor
                {
                    Ukprn = 1,
                    OrganisationIdentifier = "Identifier_1",
                    Contracts = new List<Contract>
                    {
                        new Contract
                        {
                            ContractNumber = "1",
                            StartDate = new DateTime(2018, 8, 1),
                            EndDate = new DateTime(2018, 9, 1),
                        }
                    }
                },
                new Contractor
                {
                    Ukprn = 1,
                    OrganisationIdentifier = "Identifier_1",
                    Contracts = new List<Contract>
                    {
                        new Contract
                        {
                            ContractNumber = "1.1",
                            StartDate = new DateTime(2018, 8, 1),
                        }
                    }
                },
                new Contractor
                {
                    Ukprn = 2,
                    OrganisationIdentifier = "Identifier_2",
                    Contracts = new List<Contract>
                    {
                        new Contract
                        {
                            ContractNumber = "2",
                            StartDate = new DateTime(2018, 8, 1),
                        }
                    }
                }
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            fcsMock
                .Setup(f => f.Contractors)
                .Returns(fcsContractor);
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var service = NewService(fcsMock.Object, messageCacheMock.Object);

            // act
            var fcs = await service.RetrieveAsync(CancellationToken.None);

            // assert
            Assert.Equal(2, fcs.Count);
        }

        /// <summary>
        /// Retrieves the contract allocations asynchronous.
        /// </summary>
        /// <returns>the test task</returns>
        [Fact]
        public async Task RetrieveContractAllocationsAsync()
        {
            // arrange
            var message = new TestMessage
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
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
                    ContractAllocationNumber = "100",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 2
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 2
                }
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var service = NewService(fcsMock.Object, messageCacheMock.Object);

            // act
            var fcsa = await service.RetrieveContractAllocationsAsync(CancellationToken.None);

            // assert
            Assert.Equal(4, fcsa.Count);
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync()
        {
            var message = new TestMessage
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
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123457",
                                ConRefNumber = "ESF-124"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new TestLearningDelivery[]
                        {
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123458",
                                ConRefNumber = "ESF-125"
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123459",
                                ConRefNumber = null
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123460"
                            }
                        }
                    }
                }
            };

            var allocations = new List<ContractAllocation>
            {
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29976",
                    LotReference = "01"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-124",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29976",
                    LotReference = "02"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-125",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29977",
                    LotReference = "03"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29978",
                    LotReference = "01"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 2,
                    TenderSpecReference = "itt_29978",
                    LotReference = "02"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-125",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 2
                }
            }.AsMockDbSet();

            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>()
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29978",
                    LotReference = "01",
                    SectorSubjectAreaCode = 1.0M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29978",
                    LotReference = "02",
                    SectorSubjectAreaCode = 5.2M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29977",
                    LotReference = "03",
                    SectorSubjectAreaCode = 13.10M
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_11115",
                    LotReference = "17",
                    SectorSubjectAreaCode = 1.0M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_11006",
                    LotReference = "9",
                    SectorSubjectAreaCode = 1.0M,
                },
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().HaveCount(2);
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync_NoMessage()
        {
            var message = new TestMessage();
            var allocations = new List<ContractAllocation>().AsMockDbSet();
            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>().AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().BeNull();
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync_NoConRefNumbers()
        {
            var message = new TestMessage()
            {
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 1
                }
            };
            var allocations = new List<ContractAllocation>().AsMockDbSet();
            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>().AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().BeNull();
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync_NoContractAllocations()
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
            var allocations = new List<ContractAllocation>().AsMockDbSet();
            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>().AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().BeNull();
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync_NoSectorSubjectAreaLevelCodes()
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

            var allocations = new List<ContractAllocation>
            {
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29976",
                    LotReference = "01"
                }
            }.AsMockDbSet();

            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>().AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().BeEmpty();
        }

        [Fact]
        public async Task RetrieveEligibilityRuleSectorSubjectAreaLevelAsync_CaseSensitiveCheck()
        {
            var message = new TestMessage
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
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123457",
                                ConRefNumber = "esf-124"
                            }
                        }
                    },
                    new TestLearner()
                    {
                        LearningDeliveries = new TestLearningDelivery[]
                        {
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123458",
                                ConRefNumber = "ESF-125"
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123459",
                                ConRefNumber = null
                            },
                            new TestLearningDelivery
                            {
                                LearnAimRef = "123460"
                            }
                        }
                    }
                }
            };

            var allocations = new List<ContractAllocation>
            {
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29976",
                    LotReference = "01"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-124",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29976",
                    LotReference = "02"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "esf-125",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29977",
                    LotReference = "03"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 1,
                    TenderSpecReference = "itt_29978",
                    LotReference = "01"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "ESF-123",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                    DeliveryUKPRN = 2,
                    TenderSpecReference = "itt_29978",
                    LotReference = "02"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "esf-125",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4),
                    DeliveryUKPRN = 2
                }
            }.AsMockDbSet();

            var esfSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>()
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29978",
                    LotReference = "01",
                    SectorSubjectAreaCode = 1.0M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29978",
                    LotReference = "02",
                    SectorSubjectAreaCode = 5.2M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_29977",
                    LotReference = "03",
                    SectorSubjectAreaCode = 13.10M
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_11115",
                    LotReference = "17",
                    SectorSubjectAreaCode = 1.0M,
                    MinLevelCode = "1",
                    MaxLevelCode = "2"
                },
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "itt_11006",
                    LotReference = "9",
                    SectorSubjectAreaCode = 1.0M,
                },
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations);
            fcsMock
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevel)
                .Returns(esfSubjectAreaLevels);

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock
                .SetupGet(mc => mc.Item)
                .Returns(message);

            var fcsa = await this.NewService(fcsMock.Object, messageCacheMock.Object).RetrieveEligibilityRuleSectorSubjectAreaLevelAsync(CancellationToken.None);
            fcsa.Should().HaveCount(2);
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
