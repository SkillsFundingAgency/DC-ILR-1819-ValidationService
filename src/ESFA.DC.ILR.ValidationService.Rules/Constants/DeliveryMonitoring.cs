namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of delivery montioring
    /// don't ask me why, they're all numbers...
    /// </summary>
    public static class DeliveryMonitoring
    {
        /// <summary>
        /// in receipt of low wages
        /// </summary>
        public const string InReceiptOfLowWages = "LDM363";

        /// <summary>
        /// steel industries redundancy training
        /// </summary>
        public const string SteelIndustriesRedundancyTraining = "LDM347";

        /// <summary>
        /// (DWP) mandated to skills training
        /// </summary>
        public const string MandationToSkillsTraining = "LDM318";

        public static class Types
        {
            /// <summary>
            /// source of funding
            /// </summary>
            public const string SourceOfFunding = "SOF";

            /// <summary>
            /// full or co funding indicator
            /// </summary>
            public const string FullOrCoFundingIndicator = "FFI";

            /// <summary>
            /// eligibility for enhanced apprenticeship funding
            /// </summary>
            public const string EligibilityForEnhancedApprenticeshipFunding = "EEF";

            /// <summary>
            /// restart indicator
            /// </summary>
            public const string RestartIndicator = "RES";

            /// <summary>
            /// learning support funding
            /// </summary>
            public const string LearningSupportFunding = "LSF";

            /// <summary>
            /// advanced learner loans indicator
            /// </summary>
            public const string AdvancedLearnerLoansIndicator = "ADL";

            /// <summary>
            /// advanced learner loans bursary funding
            /// </summary>
            public const string AdvancedLearnerLoansBursaryFunding = "ALB";

            /// <summary>
            /// community learning provision type
            /// </summary>
            public const string CommunityLearningProvisionType = "ASL";

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
            public const string ApprenticeshipContractType = "ACT";
        }
    }
}
