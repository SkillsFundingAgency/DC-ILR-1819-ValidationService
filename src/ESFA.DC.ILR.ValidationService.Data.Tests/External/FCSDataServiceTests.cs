using ESFA.DC.ILR.ValidationService.Data.External.FCS;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Tests.External
{
    public class FCSDataServiceTests
    {
        [Fact]
        public void ConRefNumberExists_True()
        {
            var conRefNumber = "100";

            var allocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4)
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "200",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "201",
                    FundingStreamCode = "Code1",
                    FundingStreamPeriodCode = "PeriodCode1",
                    Period = "R01",
                    StartDate = new DateTime(2018, 8, 4)
                }
            };

            var fcsContracts = new List<FcsContract>
            {
                new FcsContract
                {
                    ContractNumber = "Contract1",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                },
                new FcsContract
                {
                    ContractNumber = "Contract2",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContracts).Returns(fcsContracts);
            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).ConRefNumberExists(conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConRefNumberExists_False()
        {
            var conRefNumber = "100";

            var fcsContracts = new List<FcsContract>();
            var allocations = new List<FcsContractAllocation>();

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContracts).Returns(fcsContracts);
            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).ConRefNumberExists(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FundingRelationshipFCTExists_True()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode1", "PeriodCode3" };

            var allocations = new List<FcsContractAllocation>
                    {
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "100",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode1",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 1),
                             EndDate = new DateTime(2018, 8, 3),
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "101",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode1",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 4)
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "200",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode2",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 1),
                             EndDate = new DateTime(2018, 8, 3),
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "201",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode3",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 4)
                         }
            };

            var fcsContracts = new List<FcsContract>
            {
                new FcsContract
                {
                    ContractNumber = "Contract1",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                },
                new FcsContract
                {
                    ContractNumber = "Contract2",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContracts).Returns(fcsContracts);
            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeTrue();
        }

        [Fact]
        public void FundingRelationshipFCTExists_False()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode10" };

            var allocations = new List<FcsContractAllocation>
                    {
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "100",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode1",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 1),
                             EndDate = new DateTime(2018, 8, 3),
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "101",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode1",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 4)
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "200",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode2",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 1),
                             EndDate = new DateTime(2018, 8, 3),
                         },
                         new FcsContractAllocation
                         {
                             ContractAllocationNumber = "201",
                             FundingStreamCode = "Code1",
                             FundingStreamPeriodCode = "PeriodCode3",
                             Period = "R01",
                             StartDate = new DateTime(2018, 8, 4)
                         }
            };

            var fcsContracts = new List<FcsContract>
            {
                new FcsContract
                {
                    ContractNumber = "Contract1",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                },
                new FcsContract
                {
                    ContractNumber = "Contract2",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2018, 8, 3),
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContracts).Returns(fcsContracts);
            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeFalse();
        }

        [Fact]
        public void FundingRelationshipFCTExists_False_Null()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode1" };

            var fcsContracts = new List<FcsContractAllocation>();

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(fcsContracts);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeFalse();
        }

        private FCSDataService NewService(IExternalDataCache externalDataCache)
        {
            return new FCSDataService(externalDataCache);
        }
    }
}