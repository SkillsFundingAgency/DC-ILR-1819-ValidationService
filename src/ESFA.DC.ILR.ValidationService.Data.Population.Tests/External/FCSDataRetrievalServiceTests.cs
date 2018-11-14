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

        private FCSDataRetrievalService NewService(IFcsContext fcs = null, ICache<IMessage> messageCache = null)
        {
            return new FCSDataRetrievalService(fcs, messageCache);
        }
    }
}
