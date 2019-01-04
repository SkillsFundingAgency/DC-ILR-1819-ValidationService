using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars data service
    /// </summary>
    public interface ILARSDataService
    {
        /// <summary>
        /// Gets the deliveries for.
        /// </summary>
        /// <param name="forThisAimRef">this aim reference.</param>
        /// <returns>a collection of lars learning deliveries for this learning aim reference</returns>
        IReadOnlyCollection<ILARSLearningDelivery> GetDeliveriesFor(string forThisAimRef);

        /// <summary>
        /// Gets the validities for.
        /// </summary>
        /// <param name="forThisAimRef">this aim reference.</param>
        /// <returns>a collection of lars 'validities' for this learning aim reference</returns>
        IReadOnlyCollection<ILARSValidity> GetValiditiesFor(string forThisAimRef);

        /// <summary>
        /// Gets the standard validity for.
        /// </summary>
        /// <param name="thisStandardCode">this standard code.</param>
        /// <returns>a LARS Standard Validity</returns>
        ILARSStandardValidity GetStandardValidityFor(int thisStandardCode);

        /// <summary>
        /// Determines whether [has known learn direct class system code 3 for] [the specified this learn aim reference].
        /// </summary>
        /// <param name="thisLearnAimRef">The this learn aim reference.</param>
        /// <returns>
        ///   <c>true</c> if [has known learn direct class system code 3 for] [the specified this learn aim reference]; otherwise, <c>false</c>.
        /// </returns>
        bool HasKnownLearnDirectClassSystemCode3For(string thisLearnAimRef);

        string GetNotionalNVQLevelv2ForLearnAimRef(string learnAimRef);

        bool EffectiveDatesValidforLearnAimRef(string learnAimRef, DateTime date);

        bool EnglishPrescribedIdsExistsforLearnAimRef(string learnAimRef, HashSet<int?> engPrscIDs);

        bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool FrameWorkComponentTypeExistsInFrameworkAims(string learnAimRef, HashSet<int?> frameworkTypeComponents);

        bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool LearnAimRefExists(string learnAimRef);

        bool LearnAimRefExistsForLearningDeliveryCategoryRef(string learnAimRef, int categoryRef);

        bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level);

        bool NotionalNVQLevelV2MatchForLearnAimRefAndLevel(string learnAimRef, string level);

        bool NotionalNVQLevelV2MatchForLearnAimRefAndLevels(string learnAimRef, IEnumerable<string> levels);

        bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel3PercentForLearnAimRefAndDateAndPercentValue(string learnAimRef, DateTime learnStartDate, decimal percentValue);

        bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef);

        bool LearnDirectClassSystemCode2MatchForLearnAimRef(string learnAimRef);

        bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills);

        bool BasicSkillsMatchForLearnAimRefAndStartDate(IEnumerable<int> basicSkillsType, string learnAimRef, DateTime learnStartDate);

        bool LearnStartDateGreaterThanFrameworkEffectiveTo(DateTime learnStartDate, int? progType, int? fWorkCode, int? pwayCode);

        bool DD04DateGreaterThanFrameworkAimEffectiveTo(DateTime? dd04Date, string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(DateTime? origLearnStartDate, string learnAimRef, string validityCategory);

        bool LearnStartDateGreaterThanStandardsEffectiveTo(int? stdCode, DateTime learnStartDate);

        bool HasAnyLearningDeliveryForLearnAimRefAndTypes(string learnAimRef, IEnumerable<string> types);

        bool OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(
            DateTime? origLearnStartDate,
            string learnAimRef,
            IEnumerable<string> categoriesHashSet);
    }
}
