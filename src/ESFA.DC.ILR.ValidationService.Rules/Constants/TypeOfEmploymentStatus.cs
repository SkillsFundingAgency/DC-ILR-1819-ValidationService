namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of employment status
    /// </summary>
    public static class TypeOfEmploymentStatus
    {
        /// <summary>
        /// in paid employment
        /// </summary>
        public const int InPaidEmployment = 10;

        /// <summary>
        /// not employed, seeking and available
        /// </summary>
        public const int NotEmployedSeekingAndAvailable = 11;

        /// <summary>
        /// not employed, not seeking or not available
        /// </summary>
        public const int NotEmployedNotSeekingOrNotAvailable = 12;

        /// <summary>
        /// not known (or not) provided
        /// </summary>
        public const int NotKnownProvided = 98;
    }
}
