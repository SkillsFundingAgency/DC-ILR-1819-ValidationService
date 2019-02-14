﻿using ESFA.DC.ILR.ValidationService.Data.Interface;
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
        [InlineData(TypeOfIntegerCodedLookup.Accom, 1)]
        [InlineData(TypeOfIntegerCodedLookup.AimType, 4)]
        [InlineData(TypeOfIntegerCodedLookup.CompStatus, 4)]
        [InlineData(TypeOfIntegerCodedLookup.ContType, 2)]
        [InlineData(TypeOfIntegerCodedLookup.ELQ, 4)]
        [InlineData(TypeOfIntegerCodedLookup.EmpStat, 4)]
        [InlineData(TypeOfIntegerCodedLookup.Ethnicity, 19)]
        [InlineData(TypeOfIntegerCodedLookup.FINTYPE, 4)]
        [InlineData(TypeOfIntegerCodedLookup.FundLev, 7)]
        [InlineData(TypeOfIntegerCodedLookup.FundModel, 8)]
        [InlineData(TypeOfIntegerCodedLookup.LLDDHealthProb, 3)]
        [InlineData(TypeOfIntegerCodedLookup.LocType, 4)]
        [InlineData(TypeOfIntegerCodedLookup.ModeStud, 4)]
        [InlineData(TypeOfIntegerCodedLookup.Outcome, 4)]
        [InlineData(TypeOfIntegerCodedLookup.PriorAttain, 14)]
        [InlineData(TypeOfIntegerCodedLookup.ProgType, 8)]
        [InlineData(TypeOfIntegerCodedLookup.SEC, 9)]
        [InlineData(TypeOfIntegerCodedLookup.SOC2000, 358)]
        [InlineData(TypeOfIntegerCodedLookup.SOC2010, 374)]
        [InlineData(TypeOfIntegerCodedLookup.SpecFee, 7)]
        [InlineData(TypeOfIntegerCodedLookup.TypeYr, 5)]
        [InlineData(TypeOfIntegerCodedLookup.WithdrawReason, 15)]
        [InlineData(TypeOfIntegerCodedLookup.WorkPlaceMode, 2)]
        public async Task SimpleLookupsArePresentAndMatchExpectedCount(TypeOfIntegerCodedLookup thisKey, int expectedCount)
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
        [InlineData(TypeOfStringCodedLookup.ApprenticeshipFinancialRecord, 7)]
        [InlineData(TypeOfStringCodedLookup.AppFinRecord, 2)]
        [InlineData(TypeOfStringCodedLookup.ContPrefType, 2)]
        [InlineData(TypeOfStringCodedLookup.Domicile, 264)]
        [InlineData(TypeOfStringCodedLookup.GCSEGrade, 35)]
        [InlineData(TypeOfStringCodedLookup.ESMType, 7)]
        [InlineData(TypeOfStringCodedLookup.LearnFAMType, 11)]
        [InlineData(TypeOfStringCodedLookup.OutGrade, 502)]
        [InlineData(TypeOfStringCodedLookup.OutType, 7)]
        [InlineData(TypeOfStringCodedLookup.Sex, 2)]
        [InlineData(TypeOfStringCodedLookup.TBFinType, 2)]
        public async Task CodedLookupsArePresentAndMatchExpectedCount(TypeOfStringCodedLookup thisKey, int expectedCount)
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
        /// Time limited lookups are present and match expected count.
        /// </summary>
        /// <param name="thisKey">The this key.</param>
        /// <param name="expectedCount">The expected count.</param>
        /// <returns>a task</returns>
        [Theory]
        [InlineData(TypeOfLimitedLifeLookup.LearnerFAM, 28)]
        [InlineData(TypeOfLimitedLifeLookup.LearningDeliveryFAM, 108)]
        [InlineData(TypeOfLimitedLifeLookup.EmpOutcome, 2)]
        [InlineData(TypeOfLimitedLifeLookup.FundComp, 4)]
        [InlineData(TypeOfLimitedLifeLookup.LLDDCat, 24)]
        [InlineData(TypeOfLimitedLifeLookup.MSTuFee, 50)]
        [InlineData(TypeOfLimitedLifeLookup.OutTypedCode, 23)]
        [InlineData(TypeOfLimitedLifeLookup.QualEnt3, 61)]
        [InlineData(TypeOfLimitedLifeLookup.TTAccom, 9)]
        public async Task TimeLimitedLookupsArePresentAndMatchExpectedCount(TypeOfLimitedLifeLookup thisKey, int expectedCount)
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
        [InlineData(TypeOfListItemLookup.OutGradeLearningAimType, 11)]
        public async Task ItemLookupsArePresentAndMatchExpectedCount(TypeOfListItemLookup thisKey, int expectedCount)
        {
            // arrange
            var internalDataCache = new InternalDataCache();

            // act
            await NewService(internalDataCache).PopulateAsync(CancellationToken.None);

            Assert.True(internalDataCache.ListItemLookups.ContainsKey(thisKey));
            Assert.Equal(expectedCount, internalDataCache.ListItemLookups[thisKey].Count);
        }

        private InternalDataCachePopulationService NewService(IInternalDataCache internalDataCache = null)
        {
            return new InternalDataCachePopulationService(internalDataCache);
        }
    }
}
