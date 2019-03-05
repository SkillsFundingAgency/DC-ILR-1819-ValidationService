namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// (types of) completion status / CompStatus
    /// </summary>
    public static class CompletionState
    {
        /// <summary>
        /// is ongoing (or intending to carry on)
        /// </summary>
        public const int IsOngoing = 1;

        /// <summary>
        /// has completed
        /// </summary>
        public const int HasCompleted = 2;

        /// <summary>
        /// has withdrawn
        /// </summary>
        public const int HasWithdrawn = 3;

        /// <summary>
        /// has temporarily withdrawn
        /// </summary>
        public const int HasTemporarilyWithdrawn = 6;
    }
}
