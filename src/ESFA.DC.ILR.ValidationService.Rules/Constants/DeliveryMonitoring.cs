namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of delivery montioring
    /// don't ask me why, they're all numbers...
    /// </summary>
    public static class DeliveryMonitoring
    {
        /// <summary>
        /// olass offenders in custody
        /// </summary>
        public const string OLASSOffendersInCustody = "LDM034";

        /// <summary>
        /// released (from prison) on temporary licence
        /// </summary>
        public const string ReleasedOnTemporaryLicence = "LDM328";

        /// <summary>
        /// (DWP) mandated to skills training
        /// </summary>
        public const string MandationToSkillsTraining = "LDM318";

        /// <summary>
        /// steel industries redundancy training
        /// </summary>
        public const string SteelIndustriesRedundancyTraining = "LDM347";

        /// <summary>
        /// in receipt of low wages
        /// </summary>
        public const string InReceiptOfLowWages = "LDM363";

        /// <summary>
        /// fully funded learning aim
        /// </summary>
        public const string FullyFundedLearningAim = "FFI1";

        /// <summary>
        /// co-funded learning aim
        /// </summary>
        public const string CoFundedLearningAim = "FFI2";

        public static class Types
        {
            /// <summary>
            /// source of funding
            /// </summary>
            public const string SourceOfFunding = "SOF";

            /// <summary>
            /// full or co funding
            /// </summary>
            public const string FullOrCoFunding = "FFI";

            /// <summary>
            /// eligibility for enhanced apprenticeship funding
            /// </summary>
            public const string EligibilityForEnhancedApprenticeshipFunding = "EEF";

            /// <summary>
            /// restart indicator
            /// </summary>
            public const string Restart = "RES";

            /// <summary>
            /// learning support funding
            /// </summary>
            public const string LearningSupportFunding = "LSF";

            /// <summary>
            /// advanced learner loans indicator
            /// </summary>
            public const string AdvancedLearnerLoan = "ADL";

            /// <summary>
            /// advanced learner loans bursary funding
            /// </summary>
            public const string AdvancedLearnerLoansBursaryFunding = "ALB";

            /// <summary>
            /// community learning provision type
            /// </summary>
            public const string CommunityLearningProvision = "ASL";

            /// <summary>
            /// family english, maths and language
            /// </summary>
            public const string FamilyEnglishMathsAndLanguage = "FLN";

            /// <summary>
            /// learning (delivery monitoring)
            /// </summary>
            public const string Learning = "LDM";

            /// <summary>
            /// national skills academy
            /// </summary>
            public const string NationalSkillsAcademy = "NSA";

            /// <summary>
            /// work programme participation
            /// </summary>
            public const string WorkProgrammeParticipation = "WPP";

            /// <summary>
            /// percentage of online delivery
            /// </summary>
            public const string PercentageOfOnlineDelivery = "POD";

            /// <summary>
            /// HE monitoring
            /// </summary>
            public const string HEMonitoring = "HEM";

            /// <summary>
            /// household situation
            /// </summary>
            public const string HouseholdSituation = "HHS";

            /// <summary>
            /// apprenticeship contract type
            /// </summary>
            public const string ApprenticeshipContract = "ACT";
        }
    }
}
