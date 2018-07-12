namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSDataService
    {
        bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);

        bool LearnAimRefExists(string learnAimRef);

        bool LearnAimRefExistsForLearningDeliveryCategoryRef(string learnAimRef, int categoryRef);

        bool NotionalNVQLevelMatchForLearnAimRef(string learnAimRef, string level);

        bool NotionalNVQLevelV2MatchForLearnAimRef(string learnAimRef, string level);

        bool FullLevel2EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool FullLevel3EntitlementCategoryMatchForLearnAimRef(string learnAimRef, int level);

        bool LearnDirectClassSystemCode1MatchForLearnAimRef(string learnAimRef);

        bool BasicSkillsMatchForLearnAimRef(string learnAimRef, int basicSkills);
    }
}
