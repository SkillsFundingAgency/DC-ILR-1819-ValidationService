using System;

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
        /// Gets the maximum traning duration.
        /// </summary>
        public static TimeSpan MaximumTrainingDuration => new TimeSpan(182, 0, 0, 0);

        /// <summary>
        /// Gets the mininum viable training start date.
        /// </summary>
        public static DateTime MininumViableTrainingStartDate => new DateTime(2015, 08, 01);

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

        /// <summary>
        /// Determines whether [is viable apprenticeship] [for the specified start (date)].
        /// </summary>
        /// <param name="forThisStart">For this start.</param>
        /// <returns>
        ///   <c>true</c> if [is viable apprenticeship] [for the specified start (date)]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsViableApprenticeship(DateTime forThisStart) => forThisStart >= MininumViableTrainingStartDate;

        /// <summary>
        /// Within maxmimum training duration.
        /// </summary>
        /// <param name="fromStart">From start.</param>
        /// <param name="toFinish">To finish.</param>
        /// <returns>
        ///   <c>true</c> if [within maxmimum training duration] [from start to finish]; otherwise, <c>false</c>.
        /// </returns>
        public static bool WithinMaxmimumTrainingDuration(DateTime fromStart, DateTime toFinish) => (toFinish - fromStart) <= MaximumTrainingDuration;
    }
}
