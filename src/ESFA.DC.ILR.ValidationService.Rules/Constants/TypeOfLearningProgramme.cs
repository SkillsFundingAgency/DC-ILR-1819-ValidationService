namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of learning programme
    /// </summary>
    public static class TypeOfLearningProgramme
    {
        /// <summary>
        /// The advanced level apprenticeship
        /// </summary>
        public const int AdvancedLevelApprenticeship = 2;

        /// <summary>
        /// The intermediate level apprenticeship
        /// </summary>
        public const int IntermediateLevelApprenticeship = 3;

        /// <summary>
        /// The higher apprenticeship level 4
        /// </summary>
        public const int HigherApprenticeshipLevel4 = 20;

        /// <summary>
        /// The higher apprenticeship level 5
        /// </summary>
        public const int HigherApprenticeshipLevel5 = 21;

        /// <summary>
        /// The higher apprenticeship level 6
        /// </summary>
        public const int HigherApprenticeshipLevel6 = 22;

        /// <summary>
        /// The higher apprenticeship level 7 plus
        /// </summary>
        public const int HigherApprenticeshipLevel7Plus = 23;

        /// <summary>
        /// The traineeship
        /// </summary>
        public const int Traineeship = 24;

        /// <summary>
        /// The apprenticeship standard
        /// </summary>
        public const int ApprenticeshipStandard = 25;

        /// <summary>
        /// Gets as a set.
        /// </summary>
        public static int[] AsASet => new[]
        {
            AdvancedLevelApprenticeship,
            IntermediateLevelApprenticeship,
            HigherApprenticeshipLevel4,
            HigherApprenticeshipLevel5,
            HigherApprenticeshipLevel6,
            HigherApprenticeshipLevel7Plus,
            Traineeship,
            ApprenticeshipStandard
        };
    }
}
