using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class InternalDataCachePopulationServiceTests
    {
        [Fact]
        public async Task Populate_AcademicYear()
        {
            var internalDataCache = new InternalDataCache();

            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            var yearStart = new DateTime(2018, 8, 1);

            internalDataCache.AcademicYear.Start.Should().BeSameDateAs(yearStart);
        }

        /// <summary>
        /// Simple lookups are present and match expected count.
        /// </summary>
        /// <param name="thisKey">this key.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>a task</returns>
        [Theory]
        [InlineData(LookupSimpleKey.Accom, 1)]
        [InlineData(LookupSimpleKey.AimType, 4)]
        [InlineData(LookupSimpleKey.CompStatus, 4)]
        [InlineData(LookupSimpleKey.ContType, 2)]
        [InlineData(LookupSimpleKey.ELQ, 4)]
        [InlineData(LookupSimpleKey.EmpStat, 4)]
        [InlineData(LookupSimpleKey.Ethnicity, 19)]
        [InlineData(LookupSimpleKey.FINTYPE, 4)]
        [InlineData(LookupSimpleKey.FundLev, 7)]
        [InlineData(LookupSimpleKey.FundModel, 8)]
        [InlineData(LookupSimpleKey.LLDDHealthProb, 3)]
        [InlineData(LookupSimpleKey.LocType, 4)]
        [InlineData(LookupSimpleKey.ModeStud, 4)]
        [InlineData(LookupSimpleKey.Outcome, 4)]
        [InlineData(LookupSimpleKey.PriorAttain, 14)]
        [InlineData(LookupSimpleKey.ProgType, 8)]
        [InlineData(LookupSimpleKey.SEC, 9)]
        [InlineData(LookupSimpleKey.SOC2000, 358)]
        [InlineData(LookupSimpleKey.SpecFee, 7)]
        [InlineData(LookupSimpleKey.TypeYr, 5)]
        [InlineData(LookupSimpleKey.WithdrawReason, 15)]
        [InlineData(LookupSimpleKey.WorkPlaceMode, 2)]
        public async Task SimpleLookupsArePresentAndMatchExpectedCount(LookupSimpleKey thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            // assert
            Assert.True(internalDataCache.SimpleLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.SimpleLookups[thisKey].Count);
        }

        /// <summary>
        /// Coded lookups are present and match expected count.
        /// </summary>
        /// <param name="thisKey">The this key.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>a task</returns>
        [Theory]
        [InlineData(LookupCodedKey.AppFinRecord, 2)]
        [InlineData(LookupCodedKey.ContPrefType, 2)]
        [InlineData(LookupCodedKey.Domicile, 264)]
        [InlineData(LookupCodedKey.EngGrade, 35)]
        [InlineData(LookupCodedKey.ESMType, 7)]
        [InlineData(LookupCodedKey.LearnFAMType, 11)]
        [InlineData(LookupCodedKey.OutGrade, 502)]
        [InlineData(LookupCodedKey.OutType, 7)]
        [InlineData(LookupCodedKey.Sex, 2)]
        [InlineData(LookupCodedKey.TBFinType, 2)]
        public async Task CodedLookupsArePresentAndMatchExpectedCount(LookupCodedKey thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            // assert
            Assert.True(internalDataCache.CodedLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.CodedLookups[thisKey].Count);
        }

        /// <summary>
        /// Complx coded lookups are present and match expected count.
        /// </summary>
        /// <param name="thisKey">The this key.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>a task</returns>
        [Theory]
        [InlineData(LookupCodedKeyDictionary.ApprenticeshipFinancialRecord, 2)]
        [InlineData(LookupCodedKeyDictionary.LearnerFAM, 11)]
        public async Task CodedDictionaryLookupsArePresentAndMatchExpectedCount(LookupCodedKeyDictionary thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            // assert
            Assert.True(internalDataCache.CodedDictionaryLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.CodedDictionaryLookups[thisKey].Count);
        }

        /// <summary>
        /// Time limited lookups are present and match expected count.
        /// </summary>
        /// <param name="thisKey">The this key.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>a task</returns>
        [Theory]
        [InlineData(LookupTimeRestrictedKey.EmpOutcome, 2)]
        [InlineData(LookupTimeRestrictedKey.FundComp, 4)]
        [InlineData(LookupTimeRestrictedKey.LLDDCat, 24)]
        [InlineData(LookupTimeRestrictedKey.MSTuFee, 50)]
        [InlineData(LookupTimeRestrictedKey.OutTypedCode, 23)]
        [InlineData(LookupTimeRestrictedKey.QualEnt3, 61)]
        [InlineData(LookupTimeRestrictedKey.TTAccom, 9)]
        public async Task TimeLimitedLookupsArePresentAndMatchExpectedCount(LookupTimeRestrictedKey thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            // assert
            Assert.True(internalDataCache.LimitedLifeLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.LimitedLifeLookups[thisKey].Count);
        }

        [Theory]
        [InlineData(LookupComplexKey.LearnDelFAMType, 16)]
        public async Task ComplexLookupsArePresentAndMatchExpectedCount(LookupComplexKey thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            Assert.True(internalDataCache.CodedComplexLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.CodedComplexLookups[thisKey].Count);
        }

        private InternalDataCachePopulationService NewService(IInternalDataCache internalDataCache = null)
        {
            return new InternalDataCachePopulationService(internalDataCache);
        }
    }
}
