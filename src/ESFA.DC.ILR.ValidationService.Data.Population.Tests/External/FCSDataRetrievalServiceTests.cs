using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using FluentAssertions;
using Moq;
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
        public async Task ReteiveAsync()
        {
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
                            ContractAllocations = new List<ContractAllocation>
                            {
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "100",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 1),
                                    EndDate = new DateTime(2018, 8, 3),
                                },
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "101",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 4)
                                }
                            }
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
                            ContractAllocations = new List<ContractAllocation>
                            {
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "100",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 1),
                                    EndDate = new DateTime(2018, 8, 3),
                                },
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "101",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 4)
                                }
                            }
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
                            ContractAllocations = new List<ContractAllocation>
                            {
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "100",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 1),
                                    EndDate = new DateTime(2018, 8, 3),
                                },
                                new ContractAllocation
                                {
                                    ContractAllocationNumber = "101",
                                    FundingStreamCode = "Code1",
                                    FundingStreamPeriodCode = "PeriodCode1",
                                    Period = "R01",
                                    StartDate = new DateTime(2018, 8, 4)
                                }
                            }
                        }
                    }
                }
            }.AsMockDbSet();

            var fcsMock = new Mock<IFcsContext>();
            var messageCacheMock = new Mock<ICache<IMessage>>();

            fcsMock.Setup(f => f.Contractors).Returns(fcsContractor);
            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var fcs = await NewService(fcsMock.Object, messageCacheMock.Object).RetrieveAsync(CancellationToken.None);

            fcs.Should().HaveCount(2);
            fcs.SelectMany(c => c.FcsContractAllocations).Should().HaveCount(4);
        }

        private FCSDataRetrievalService NewService(IFcsContext fcs = null, ICache<IMessage> messageCache = null)
        {
            return new FCSDataRetrievalService(fcs, messageCache);
        }
    }
}
