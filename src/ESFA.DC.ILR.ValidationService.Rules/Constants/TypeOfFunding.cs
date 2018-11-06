namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of funding / Fund Models
    /// </summary>
    public static class TypeOfFunding
    {
        /// <summary>
        /// Community learning
        /// </summary>
        public const int CommunityLearning = 10;

        /// <summary>
        /// Age 16 to 19 excluding apprenticeships
        /// </summary>
        public const int Age16To19ExcludingApprenticeships = 25;

        /// <summary>
        /// Adult skills
        /// </summary>
        public const int AdultSkills = 35;

        /// <summary>
        /// Apprenticeships from 1 May 2017
        /// </summary>
        public const int ApprenticeshipsFrom1May2017 = 36;

        /// <summary>
        /// The european social fund
        /// </summary>
        public const int EuropeanSocialFund = 70;

        /// <summary>
        /// Other adult
        /// </summary>
        public const int OtherAdult = 81;

        /// <summary>
        /// Other 16 to 19
        /// </summary>
        public const int Other16To19 = 82;

        /// <summary>
        /// Not funded by ESFA
        /// </summary>
        public const int NotFundedByESFA = 99;

        /// <summary>
        /// Gets as a set.
        /// </summary>
        public static int[] AsASet => new[]
        {
            CommunityLearning,
            Age16To19ExcludingApprenticeships,
            AdultSkills,
            ApprenticeshipsFrom1May2017,
            EuropeanSocialFund,
            OtherAdult,
            Other16To19,
            NotFundedByESFA
        };

        /// <summary>
        /// Gets as a funded set.
        /// </summary>
        public static int[] AsAFundedSet => new[]
        {
            CommunityLearning,
            Age16To19ExcludingApprenticeships,
            AdultSkills,
            ApprenticeshipsFrom1May2017,
            EuropeanSocialFund,
            OtherAdult,
            Other16To19
        };

        /// <summary>
        /// Gets as a non funded set.
        /// </summary>
        public static int[] AsANonFundedSet => new[]
        {
            NotFundedByESFA
        };
    }
}
