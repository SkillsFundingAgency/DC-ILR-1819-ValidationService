using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS
{
    public class LARSDataService : ILARSDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        public LARSDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        /// <summary>
        /// Gets the deliveries for.
        /// </summary>
        /// <param name="forThisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning deliveries for this learning aim reference</returns>
        public IReadOnlyCollection<ILARSLearningDelivery> GetDeliveriesFor(string forThisAimRef)
        {
            return _externalDataCache.LearningDeliveries?.Values
                .Where(x => x.LearnAimRef.CaseInsensitiveEquals(forThisAimRef))
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Gets the lars learning delivery for.
        /// </summary>
        /// <param name="learnAimRef">this learn aim reference.</param>
        /// <returns>a lars learning delivery record for this learning aim reference</returns>
        public ILARSLearningDelivery GetLearningDeliveryForLearnAimRef(string learnAimRef)
        {
            return _externalDataCache.LearningDeliveries?.Values?
                 .Where(e => e.LearnAimRef.CaseInsensitiveEquals(learnAimRef))?
                 .FirstOrDefault();
        }

        /// <summary>
        /// Gets the validities for.
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a collection of lars 'validities' for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSValidity> GetValiditiesFor(string thisAimRef)
        {
            return _externalDataCache.LearningDeliveries.Values
                .SelectMany(x => x.LARSValidities.AsSafeReadOnlyList())
                .Where(x => x.LearnAimRef == thisAimRef)
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Gets the standard validity for.
        /// </summary>
        /// <param name="thisStandardCode">this standard code.</param>
        /// <returns>a LARS Standard Validity</returns>
        public ILARSStandardValidity GetStandardValidityFor(int thisStandardCode)
        {
            return _externalDataCache.StandardValidities
                .Where(x => x.StandardCode == thisStandardCode)
                .FirstOrDefault();
        }

        public string GetNotionalNVQLevelv2ForLearnAimRef(string learnAimRef)
        {
            return GetDeliveriesFor(learnAimRef)?
                .Select(l => l.NotionalNVQLevelv2).FirstOrDefault();
        }

        public bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.EffectiveFrom <= date
                && (learningDelivery.EffectiveTo != null ? date <= learningDelivery.EffectiveTo : date <= DateTime.MaxValue);
        }

        public bool EnglishPrescribedIdsExistsforLearnAimRef(string learnAimRef, HashSet<int?> engPrscIDs)
        {
            return GetDeliveriesFor(learnAimRef)?.Where(e => engPrscIDs.Contains(e.EnglPrscID)).Any() ?? false;
        }

        public bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return _externalDataCache
                .Frameworks
                .Where(f => f.FrameworkAims != null)
                .SelectMany(f => f.FrameworkAims
                    .Where(fa =>
                        fa.LearnAimRef.CaseInsensitiveEquals(learnAimRef)
                        && fa.ProgType == progType
                        && fa.FworkCode == fworkCode
                        && fa.PwayCode == pwayCode))
                .Any();
        }

        public bool FrameWorkComponentTypeExistsInFrameworkAims(string learnAimRef, HashSet<int?> frameworkTypeComponents)
        {
            return _externalDataCache
                .Frameworks?
                .Where(f => f.FrameworkAims != null)
                .SelectMany(f => f.FrameworkAims
                    .Where(fa =>
                        fa.LearnAimRef.CaseInsensitiveEquals(learnAimRef)
                        && (fa.FrameworkComponentType.HasValue
                            && frameworkTypeComponents.Contains(fa.FrameworkComponentType))))
                .Any() ?? false;
        }

        public bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                   && _externalDataCache
                        .Frameworks
                        .Where(f => f.FrameworkCommonComponents != null)
                        .SelectMany(f => f.FrameworkCommonComponents
                            .Where(fa =>
                                fa.ProgType == progType
                                && fa.FworkCode == fworkCode
                                && fa.PwayCode == pwayCode
                                && fa.CommonComponent == learningDelivery.FrameworkCommonComponent))
                        .Any();
        }

        public bool LearnAimRefExists(string learnAimRef)
        {
            return _externalDataCache.LearningDeliveries.ContainsKey(learnAimRef);
        }

        public bool LearnAimRefExistsForLearningDeliveryCategoryRef(string learnAimRef, int categoryRef)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.LearningDeliveryCategories != null
                && learningDelivery.LearningDeliveryCategories.Where(cr => cr.CategoryRef == categoryRef).Any();
        }

        public bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null && learningDelivery.NotionalNVQLevel == level;
        }

        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevel(string learnAimRef, string level)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null && learningDelivery.NotionalNVQLevelv2 == level;
        }

        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevels(string learnAimRef, IEnumerable<string> levels)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null && levels.Contains(learningDelivery.NotionalNVQLevelv2);
        }

        public bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Where(av => av.FullLevel2EntitlementCategory == level).Any();
        }

        public bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Where(av => av.FullLevel3EntitlementCategory == level).Any();
        }

        public bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Where(av =>
                       av.FullLevel3Percent == percentValue
                    && av.EffectiveFrom <= learnStartDate
                    && av.EffectiveTo >= learnStartDate).Any();
        }

        /// <summary>
        /// Determines whether [has known learn direct class system code 3 for] [the specified this learn aim reference].
        /// </summary>
        /// <param name="thisLearnAimRef">The this learn aim reference.</param>
        /// <returns>
        ///   <c>true</c> if [has known learn direct class system code 3 for] [the specified this learn aim reference]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasKnownLearnDirectClassSystemCode3For(string thisLearnAimRef)
        {
            var delivery = GetDeliveriesFor(thisLearnAimRef).FirstOrDefault();

            return It.Has(delivery)
                && delivery.LearnDirectClassSystemCode3.IsKnown();
        }

        public bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.LearnDirectClassSystemCode1.IsKnown();
        }

        public bool LearnDirectClassSystemCode2MatchForLearnAimRef(string learnAimRef)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.LearnDirectClassSystemCode2.IsKnown();
        }

        public bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Where(av => av.BasicSkills == basicSkills).Any();
        }

        public bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate)
        {
            return GetDeliveriesFor(learnAimRef)?
                .SelectMany(ld => ld.AnnualValues)?
                .Where(
                    a => (a.BasicSkillsType.HasValue
                        && basicSkillsType.ToCaseInsensitiveHashSet().Contains((int)a.BasicSkillsType))
                    && (learnStartDate >= a.EffectiveFrom
                    && (learnStartDate <= a.EffectiveTo || a.EffectiveTo == null))).Count() > 0;
        }

        public bool LearnStartDateGreaterThanFrameworkEffectiveTo(DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode)
        {
            return _externalDataCache.Frameworks
                .Where(f =>
                    f.ProgType == progType
                    && f.FworkCode == fWorkCode
                    && f.PwayCode == pwayCode
                    && f.EffectiveTo != null
                    && learnStartDate > f.EffectiveTo)
                .Any();
        }

        public bool DD04DateGreaterThanFrameworkAimEffectiveTo(DateTime? dd04Date, string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return _externalDataCache
                .Frameworks
                .Where(f => f.FrameworkAims != null)
                .SelectMany(f => f.FrameworkAims
                    .Where(fa =>
                        fa.LearnAimRef.CaseInsensitiveEquals(learnAimRef)
                        && fa.ProgType == progType
                        && fa.FworkCode == fworkCode
                        && fa.PwayCode == pwayCode
                        && fa.EffectiveTo != null
                        && dd04Date > fa.EffectiveTo))
                .Any();
        }

        public bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime? origLearnStartDate, string learnAimRef, string validityCategory)
        {
            return OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(
                origLearnStartDate,
                learnAimRef,
                new List<string>() { validityCategory });
        }

        public bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(DateTime? origLearnStartDate, string learnAimRef, IEnumerable<string> categoriesHashSet)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);
            var caseInsensitveCategoriesHashSet = categoriesHashSet.ToCaseInsensitiveHashSet();

            return learningDelivery?.LARSValidities != null
                   && learningDelivery.LARSValidities.Any(lv => lv.LearnAimRef.CaseInsensitiveEquals(learnAimRef)
                                                                && caseInsensitveCategoriesHashSet.Contains(lv.ValidityCategory)
                                                                && origLearnStartDate >= lv.StartDate
                                                                && origLearnStartDate <= lv.EndDate);
        }

        public bool LearnStartDateGreaterThanStandardsEffectiveTo(int? stdCode, DateTime learnStartDate)
        {
            return stdCode.HasValue
                   && _externalDataCache.Standards.Any(s => s.EffectiveTo != null
                                                            && learnStartDate > s.EffectiveTo
                                                            && s.StandardCode == stdCode);
        }

        public bool HasAnyLearningDeliveryForLearnAimRefAndTypes(string learnAimRef, IEnumerable<string> learnAimRefTypes)
        {
            return GetDeliveriesFor(learnAimRef)?
                .Where(ld => learnAimRefTypes.ToCaseInsensitiveHashSet().Contains(ld.LearnAimRefType))?
                .Count() > 0;
        }
    }
}
