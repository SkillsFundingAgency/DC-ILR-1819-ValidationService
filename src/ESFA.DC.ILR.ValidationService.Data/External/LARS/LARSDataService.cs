using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS
{
    public class LARSDataService : ILARSDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        public LARSDataService(IExternalDataCache externalDataCache)
        {
            _externalDataCache = externalDataCache;
        }

        public bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.EffectiveFrom <= date
                && (learningDelivery.EffectiveTo != null ? date <= learningDelivery.EffectiveTo : date <= DateTime.MaxValue);
        }

        public bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return _externalDataCache
                .Frameworks
                .Where(f => f.FrameworkAims != null)
                .SelectMany(f => f.FrameworkAims
                    .Where(fa =>
                        fa.LearnAimRef == learnAimRef
                        && fa.ProgType == progType
                        && fa.FworkCode == fworkCode
                        && fa.PwayCode == pwayCode))
                .Any();
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

        public bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef)
        {
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.LearnDirectClassSystemCode1 != "NUL";
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
            _externalDataCache.LearningDeliveries.TryGetValue(learnAimRef, out var learningDelivery);

            return learningDelivery != null
                && learningDelivery.AnnualValues != null
                && learningDelivery.AnnualValues.Where(
                    a => basicSkillsType.Contains(a.BasicSkillsType ?? -9999)
                    && (learnStartDate >= a.EffectiveFrom
                    && (learnStartDate <= a.EffectiveTo || a.EffectiveTo == null))).Count() > 0;
        }
    }
}
