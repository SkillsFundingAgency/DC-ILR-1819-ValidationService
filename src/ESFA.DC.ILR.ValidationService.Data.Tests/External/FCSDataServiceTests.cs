﻿using ESFA.DC.ILR.ValidationService.Data.External.FCS;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
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

            var allocations = new Dictionary<string, IFcsContractAllocation>()
            {
                { "100", new FcsContractAllocation { ContractAllocationNumber = "100", FundingStreamPeriodCode = "PeriodCode1" } },
                { "101", new FcsContractAllocation { ContractAllocationNumber = "101", FundingStreamPeriodCode = "PeriodCode1" } },
                { "200", new FcsContractAllocation { ContractAllocationNumber = "200", FundingStreamPeriodCode = "PeriodCode1" } },
                { "201", new FcsContractAllocation { ContractAllocationNumber = "201", FundingStreamPeriodCode = "PeriodCode1" } }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).ConRefNumberExists(conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConRefNumberExists_False()
        {
            var conRefNumber = "100";

            var allocations = new Dictionary<string, IFcsContractAllocation>();

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCacheMock.Object).ConRefNumberExists(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FundingRelationshipFCTExists_True()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode1", "PeriodCode3" };

            var allocations = new Dictionary<string, IFcsContractAllocation>
            {
                { "100", new FcsContractAllocation { ContractAllocationNumber = "100", FundingStreamPeriodCode = "PeriodCode1" } },
                { "101", new FcsContractAllocation { ContractAllocationNumber = "101", FundingStreamPeriodCode = "PeriodCode1" } },
                { "200", new FcsContractAllocation { ContractAllocationNumber = "200", FundingStreamPeriodCode = "PeriodCode2" } },
                { "201", new FcsContractAllocation { ContractAllocationNumber = "201", FundingStreamPeriodCode = "PeriodCode3" } },
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeTrue();
        }

        [Fact]
        public void FundingRelationshipFCTExists_False()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode10" };

            var allocations = new Dictionary<string, IFcsContractAllocation>
            {
                { "100", new FcsContractAllocation { ContractAllocationNumber = "100", FundingStreamPeriodCode = "PeriodCode1" } },
                { "101", new FcsContractAllocation { ContractAllocationNumber = "101", FundingStreamPeriodCode = "PeriodCode1" } },
                { "200", new FcsContractAllocation { ContractAllocationNumber = "200", FundingStreamPeriodCode = "PeriodCode2" } },
                { "201", new FcsContractAllocation { ContractAllocationNumber = "201", FundingStreamPeriodCode = "PeriodCode3" } },
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeFalse();
        }

        [Fact]
        public void FundingRelationshipFCTExists_False_Null()
        {
            var fundingStreamPeriodCodes = new List<string> { "PeriodCode1" };

            var fcsContracts = new Dictionary<string, IFcsContractAllocation>();

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(f => f.FCSContractAllocations).Returns(fcsContracts);

            NewService(externalDataCahceMock.Object).FundingRelationshipFCTExists(fundingStreamPeriodCodes).Should().BeFalse();
        }

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_DataCheck()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = "2",
                    MaxLevelCode = "4"
                }
            };

            var allocations = new Dictionary<string, IFcsContractAllocation>
            {
                {
                    conRefNumber, new FcsContractAllocation
                    {
                        ContractAllocationNumber = conRefNumber, TenderSpecReference = "tt_9972", LotReference = "01",
                        EsfEligibilityRule = new EsfEligibilityRule()
                        {
                            SectorSubjectAreaLevels = sectorSubjectAreaLevels
                        }
                    }
                },
                { "101", new FcsContractAllocation { ContractAllocationNumber = "101", TenderSpecReference = "tt_9978", LotReference = "04" } },
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCahceMock.Object)
                .GetEligibilityRuleSectorSubjectAreaLevelsFor(conRefNumber).Should().BeSameAs(sectorSubjectAreaLevels);
        }

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_NullCheck()
        {
            string conRefNumber = "ESF0002";

            var allocations = new Dictionary<string, IFcsContractAllocation>()
            {
                { conRefNumber, new FcsContractAllocation { ContractAllocationNumber = conRefNumber, TenderSpecReference = "tt_9972", LotReference = "01" } },
                { "101",  new FcsContractAllocation { ContractAllocationNumber = "101", TenderSpecReference = "tt_9978", LotReference = "04" } }
            };

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.Setup(e => e.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCacheMock.Object)
                .GetEligibilityRuleSectorSubjectAreaLevelsFor(conRefNumber).Should().BeEmpty();
        }

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_ContractAllocation_NullCheck()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = "2",
                    MaxLevelCode = "4"
                }
            };

            Dictionary<string, IFcsContractAllocation> allocations = null;

            var externalDataCacheMock = new Mock<IExternalDataCache>();

            externalDataCacheMock.Setup(e => e.FCSContractAllocations).Returns(allocations);

            NewService(externalDataCacheMock.Object)
                .GetEligibilityRuleSectorSubjectAreaLevelsFor(conRefNumber).Should().BeEmpty();
        }

        /// <summary>
        /// Get contract allocation for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("ESF0002", "tt_9972")]
        [InlineData("ESF0003", "tt_9972")]
        [InlineData("ESF0004", "tt_9972")]
        [InlineData("ESF0005", "tt_9978")]
        public void GetContractAllocationForMeetsExpectation(string candidate, string expectation)
        {
            // arrange
            var cache = GetDefaultStrictEmptyCache();
            Mock.Get(cache)
                .Setup(e => e.FCSContractAllocations)
                .Returns(GetDefaultContractAllocationTestList());

            var sut = new FCSDataService(cache);

            // act
            var result = sut.GetContractAllocationFor(candidate);

            // assert
            // TODO: fix me...
            // Assert.Equal(expectation, result.TenderSpecReference);
            expectation.Should().Be(result.TenderSpecReference);
            Mock.Get(cache).VerifyAll();
        }

        /// <summary>
        /// Get contract allocation for, with null reference returns null.
        /// </summary>
        [Fact]
        public void GetContractAllocationForWithNullReferenceReturnsNull()
        {
            // arrange
            var cache = GetDefaultStrictEmptyCache();

            Mock.Get(cache)
                .Setup(e => e.FCSContractAllocations)
                .Returns(GetDefaultContractAllocationTestList());

            var sut = new FCSDataService(cache);

            // act
            var result = sut.GetContractAllocationFor(null);

            // assert
            result.Should().BeNull();
            Mock.Get(cache).VerifyAll();
        }

        /// <summary>
        /// Gets the default contract allocation test list.
        /// </summary>
        /// <returns>the default set of contract allocations for testing</returns>
        private IReadOnlyDictionary<string, IFcsContractAllocation> GetDefaultContractAllocationTestList()
        {
            // this provides a set of test contract allocations
            return new Dictionary<string, IFcsContractAllocation>
            {
                { "ESF0002", new FcsContractAllocation { ContractAllocationNumber = "ESF0002", TenderSpecReference = "tt_9972", LotReference = "01" } },
                { "ESF0003", new FcsContractAllocation { ContractAllocationNumber = "ESF0003", TenderSpecReference = "tt_9972", LotReference = "02" } },
                { "ESF0004", new FcsContractAllocation { ContractAllocationNumber = "ESF0004", TenderSpecReference = "tt_9972", LotReference = "03" } },
                { "ESF0005", new FcsContractAllocation { ContractAllocationNumber = "ESF0005", TenderSpecReference = "tt_9978", LotReference = "01" } },
            };
        }

        /// <summary>
        /// Gets the default strict empty cache.
        /// </summary>
        /// <returns>an i external data cache</returns>
        private IExternalDataCache GetDefaultStrictEmptyCache()
        {
            // this sets up an empty by default, verifiable mocked cache with strict behaviour
            var cache = new Mock<IExternalDataCache>(MockBehavior.Strict);
            cache
                .Setup(e => e.FCSContractAllocations)
                .Returns((IReadOnlyDictionary<string, IFcsContractAllocation>)null);

            return cache.Object;
        }

        private FCSDataService NewService(IExternalDataCache externalDataCache)
        {
            return new FCSDataService(externalDataCache);
        }
    }
}