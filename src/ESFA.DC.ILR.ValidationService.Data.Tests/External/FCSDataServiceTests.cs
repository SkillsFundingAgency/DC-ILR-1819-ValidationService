using ESFA.DC.ILR.ValidationService.Data.External.FCS;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
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

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_DataCheck()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    Id = 1,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = "2",
                    MaxLevelCode = "4"
                }
            };

            var allocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = conRefNumber,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "101",
                    TenderSpecReference = "tt_9978",
                    LotReference = "04"
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);
            externalDataCahceMock.Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels).Returns(sectorSubjectAreaLevels);

            NewService(externalDataCache: externalDataCahceMock.Object)
                .GetSectorSubjectAreaLevelsForContract(conRefNumber).Equals(sectorSubjectAreaLevels);
        }

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_NullCheck()
        {
            string conRefNumber = "ESF0002";
            IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> esfEligibilityRuleSectorSubjectAreaLevels = null;
            var allocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = conRefNumber,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "101",
                    TenderSpecReference = "tt_9978",
                    LotReference = "04"
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);
            externalDataCahceMock.Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels).Returns(esfEligibilityRuleSectorSubjectAreaLevels);

            NewService(externalDataCache: externalDataCahceMock.Object)
                .GetSectorSubjectAreaLevelsForContract(conRefNumber).Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetSectorSubjectAreaLevelsForContract_ContractAllocation_NullCheck()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    Id = 1,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = "2",
                    MaxLevelCode = "4"
                }
            };

            List<FcsContractAllocation> allocations = null;

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);
            externalDataCahceMock.Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels).Returns(sectorSubjectAreaLevels);

            NewService(externalDataCache: externalDataCahceMock.Object)
                .GetSectorSubjectAreaLevelsForContract(conRefNumber).Should().BeNullOrEmpty();
        }

        [Fact]
        public void IsSectorSubjectAreaCodeExistsForContract_False()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    Id = 1,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = "2",
                    MaxLevelCode = "4"
                }
            };

            var allocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = conRefNumber,
                    TenderSpecReference = "tt_9979",
                    LotReference = "01"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "101",
                    TenderSpecReference = "tt_9978",
                    LotReference = "04"
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);
            externalDataCahceMock.Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels).Returns(sectorSubjectAreaLevels);

            NewService(externalDataCache: externalDataCahceMock.Object).IsSectorSubjectAreaCodeExistsForContract(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void IsSectorSubjectAreaCodeExistsForContract_True()
        {
            string conRefNumber = "ESF0002";
            var sectorSubjectAreaLevels = new IEsfEligibilityRuleSectorSubjectAreaLevel[]
            {
                new EsfEligibilityRuleSectorSubjectAreaLevel()
                {
                    Id = 1,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01",
                    SectorSubjectAreaCode = 13.1M,
                    MinLevelCode = null,
                    MaxLevelCode = null
                }
            };

            var allocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = conRefNumber,
                    TenderSpecReference = "tt_9972",
                    LotReference = "01"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "101",
                    TenderSpecReference = "tt_9978",
                    LotReference = "04"
                }
            };

            var externalDataCahceMock = new Mock<IExternalDataCache>();

            externalDataCahceMock.Setup(e => e.FCSContractAllocations).Returns(allocations);
            externalDataCahceMock.Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels).Returns(sectorSubjectAreaLevels);

            NewService(externalDataCache: externalDataCahceMock.Object).IsSectorSubjectAreaCodeExistsForContract(conRefNumber).Should().BeTrue();
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
            Assert.Equal(expectation, result.TenderSpecReference);
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
            Assert.Null(result);
            Mock.Get(cache).VerifyAll();
        }

        /// <summary>
        /// That matches, meets expectation.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="allocation">The allocation.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("ESF0002", "tt_9972", false)]
        [InlineData("tt_9972", "ESF0002", false)]
        [InlineData("TT_9972", "tt_9972", true)]
        [InlineData("tt_9972", "TT_9972", true)]
        [InlineData("tt_9972", "tt_9972", true)]
        [InlineData("TT_9973", "tt_9972", false)]
        [InlineData("tt_9972", "TT_9973", false)]
        [InlineData("tt_9973", "tt_9972", false)]
        [InlineData("tt_9972", "tt_9973", false)]
        public void ThatMatchesMeetsExpectation(string reference, string allocation, bool expectation)
        {
            // arrange
            var cache = GetDefaultStrictEmptyCache();
            var sut = new FCSDataService(cache);

            var referenceMock = new Mock<IEsfEligibilityRuleReferences>();
            referenceMock
                .SetupGet(x => x.TenderSpecReference)
                .Returns(reference);
            var allocatonMock = new Mock<IFcsContractAllocation>();
            allocatonMock
                .SetupGet(x => x.TenderSpecReference)
                .Returns(allocation);

            // act
            var result = sut.ThatMatches(referenceMock.Object, allocatonMock.Object);

            // assert
            Assert.Equal(expectation, result);
            Mock.Get(cache).VerifyAll();
        }

        /// <summary>
        /// Gets the default contract allocation test list.
        /// </summary>
        /// <returns>the default set of contract allocations for testing</returns>
        internal IReadOnlyCollection<IFcsContractAllocation> GetDefaultContractAllocationTestList()
        {
            // this provides a set of test contract allocations
            return new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "ESF0002",
                    TenderSpecReference = "tt_9972",
                    LotReference = "01"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "ESF0003",
                    TenderSpecReference = "tt_9972",
                    LotReference = "02"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "ESF0004",
                    TenderSpecReference = "tt_9972",
                    LotReference = "03"
                },
                new FcsContractAllocation
                {
                    ContractAllocationNumber = "ESF0005",
                    TenderSpecReference = "tt_9978",
                    LotReference = "01"
                }
            };
        }

        /// <summary>
        /// Gets the default strict empty cache.
        /// </summary>
        /// <returns>an i external data cache</returns>
        internal IExternalDataCache GetDefaultStrictEmptyCache()
        {
            // this sets up an empty by default, verifiable mocked cache with strict behaviour
            var cache = new Mock<IExternalDataCache>(MockBehavior.Strict);
            cache
                .Setup(e => e.FCSContractAllocations)
                .Returns((IReadOnlyCollection<IFcsContractAllocation>)null);
            cache
                .Setup(e => e.ESFEligibilityRuleEmploymentStatuses)
                .Returns((IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>)null);
            cache
                .Setup(e => e.EsfEligibilityRuleSectorSubjectAreaLevels)
                .Returns((IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel>)null);
            cache
                .Setup(e => e.ESFEligibilityRuleLocalAuthorities)
                .Returns((IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority>)null);

            return cache.Object;
        }

        internal FCSDataService NewService(IExternalDataCache externalDataCache)
        {
            return new FCSDataService(externalDataCache);
        }
    }
}