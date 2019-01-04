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
            It.IsNull(externalDataCache)
                .AsGuard<ArgumentNullException>(nameof(externalDataCache));

            _externalDataCache = externalDataCache;
        }

        /// <summary>
        /// Gets the deliveries for.
        /// this routine has been deprecated on the contract
        /// so don't use it; use 'get delivery for' instead...
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning deliveries for this learning aim reference</returns>
        public IReadOnlyCollection<ILARSLearningDelivery> GetDeliveriesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return It.Has(delivery)
                ? new[] { delivery }
                : Collection.EmptyAndReadOnly<ILARSLearningDelivery>();
        }

        /// <summary>
        /// Gets the delivery for.
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a lars learning delivery for this learning aim reference
        /// </returns>
        public ILARSLearningDelivery GetDeliveryFor(string thisAimRef)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(thisAimRef, out var learningDelivery);

            return learningDelivery;
        }

        /// <summary>
        /// Gets the categories for.
        /// </summary>
        /// <param name="thisAimRef">this aim reference.</param>
        /// <returns>
        /// a collection of lars learning categories for this learning aim reference
        /// </returns>
        public IReadOnlyCollection<ILARSLearningCategory> GetCategoriesFor(string thisAimRef)
        {
            var delivery = GetDeliveryFor(thisAimRef);

            return It.Has(delivery)
                ? delivery.LearningDeliveryCategories.AsSafeReadOnlyList()
                : Collection.EmptyAndReadOnly<ILARSLearningCategory>();
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
            var delivery = GetDeliveryFor(thisAimRef);

            return It.Has(delivery)
                ? delivery.LARSValidities.AsSafeReadOnlyList()
                : Collection.EmptyAndReadOnly<ILARSValidity>();
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
            return GetDeliveryFor(learnAimRef)?.NotionalNVQLevelv2;
        }

        public bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.EffectiveFrom <= date
                && (learningDelivery.EffectiveTo != null ? date <= learningDelivery.EffectiveTo : date <= DateTime.MaxValue);
        }

        public bool EnglishPrescribedIdsExistsforLearnAimRef(string learnAimRef, HashSet<int?> engPrscIDs)
        {
            return engPrscIDs.Contains(GetDeliveryFor(learnAimRef)?.EnglPrscID);
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
            var learningDelivery = GetDeliveryFor(learnAimRef);

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
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.LearningDeliveryCategories != null
                && learningDelivery.LearningDeliveryCategories.Where(cr => cr.CategoryRef == categoryRef).Any();
        }

        public bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.NotionalNVQLevel == level;
        }

        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevel(string learnAimRef, string level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.NotionalNVQLevelv2 == level;
        }

        public bool NotionalNVQLevelV2MatchForLearnAimRefAndLevels(string learnAimRef, IEnumerable<string> levels)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && levels.Contains(learningDelivery.NotionalNVQLevelv2);
        }

        public bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Any(av => av.FullLevel2EntitlementCategory == level);
        }

        public bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Any(av => av.FullLevel3EntitlementCategory == level);
        }

        public bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Any(av =>
                        av.FullLevel3Percent == percentValue
                            && av.EffectiveFrom <= learnStartDate
                            && av.EffectiveTo >= learnStartDate);
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
            var delivery = GetDeliveryFor(thisLearnAimRef);

            return It.Has(delivery)
                && delivery.LearnDirectClassSystemCode3.IsKnown();
        }

        public bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.LearnDirectClassSystemCode1.IsKnown();
        }

        public bool LearnDirectClassSystemCode2MatchForLearnAimRef(string learnAimRef)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.LearnDirectClassSystemCode2.IsKnown();
        }

        public bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues
                    .Any(av => av.BasicSkills == basicSkills);
        }

        public bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate)
        {
            var delivery = GetDeliveryFor(learnAimRef);

            return It.Has(delivery)
                && delivery.AnnualValues
                    .SafeAny(a => a.BasicSkillsType.HasValue
                        && basicSkillsType.Contains((int)a.BasicSkillsType)
                        && learnStartDate >= a.EffectiveFrom
                        && learnStartDate <= a.EffectiveTo);
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

        public bool OrigLearnStartDateBetweenStartAndEndDateForValidityApprenticeships(DateTime? origLearnStartDate, string learnAimRef)
        {
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return learningDelivery != null
                   && learningDelivery.LARSValidities != null
                   && learningDelivery.LARSValidities.Any(lv => lv.LearnAimRef.CaseInsensitiveEquals(learnAimRef)
                                                                && lv.ValidityCategory == "APPRENTICESHIPS"
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
            var learningDelivery = GetDeliveryFor(learnAimRef);

            return It.Has(learningDelivery)
                && learnAimRefTypes.ToCaseInsensitiveHashSet().Contains(learningDelivery.LearnAimRefType);
        }
    }
}
