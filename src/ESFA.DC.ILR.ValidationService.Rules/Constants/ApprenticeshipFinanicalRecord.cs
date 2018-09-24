namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of apprenticeship financial record
    /// contains the list of valid type code combinations
    /// </summary>
    public class ApprenticeshipFinanicalRecord
    {
        /// <summary>
        /// The total training price
        /// </summary>
        public const string TotalTrainingPrice = "TNP1";

        /// <summary>
        /// The total assessment price
        /// </summary>
        public const string TotalAssessmentPrice = "TNP2";

        /// <summary>
        /// The residual training price
        /// </summary>
        public const string ResidualTrainingPrice = "TNP3";

        /// <summary>
        /// The residual assessment price
        /// </summary>
        public const string ResidualAssessmentPrice = "TNP4";

        /// <summary>
        /// The training payment
        /// </summary>
        public const string TrainingPayment = "PMR1";

        /// <summary>
        /// The assessment payment
        /// </summary>
        public const string AssessmentPayment = "PMR2";

        /// <summary>
        /// The employer payment reimbursed by provider
        /// </summary>
        public const string EmployerPaymentReimbursedByProvider = "PMR3";

        public static class Types
        {
            /// <summary>
            /// The total negotiated price
            /// </summary>
            public const string TotalNegotiatedPrice = "TNP";

            /// <summary>
            /// The payment record
            /// </summary>
            public const string PaymentRecord = "PMR";
        }
    }
}
