namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of apprenticeship financial record
    /// contains the list of valid type code combinations
    /// </summary>
    public static class ApprenticeshipFinancialRecord
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

        /// <summary>
        /// Total Negotiated Price Codes / TNP Codes
        /// </summary>
        public static class TotalNegotiatedPriceCodes
        {
            /// <summary>
            /// Total training price / 1
            /// </summary>
            public const int TotalTrainingPrice = 1;

            /// <summary>
            /// Total training price / 1
            /// </summary>
            public const int TotalAssessmentPrice = 2;

            /// <summary>
            /// Total assessment price / 3
            /// </summary>
            public const int ResidualTrainingPrice = 3;

            /// <summary>
            /// Residual training price / 4
            /// </summary>
            public const int ResidualAssessmentPrice = 4;

            /// <summary>
            /// Unassigned / 5
            /// </summary>
            public const int Unassigned5 = 5;

            /// <summary>
            /// Unassigned / 6
            /// </summary>
            public const int Unassigned6 = 6;

            /// <summary>
            /// Unassigned / 7
            /// </summary>
            public const int Unassigned7 = 7;

            /// <summary>
            /// Unassigned / 8
            /// </summary>
            public const int Unassigned8 = 8;

            /// <summary>
            /// Unassigned / 9
            /// </summary>
            public const int Unassigned9 = 9;

            /// <summary>
            /// Unassigned / 10
            /// </summary>
            public const int Unassigned10 = 10;
        }

        /// <summary>
        /// Payment Record Codes / PMR Codes
        /// </summary>
        public static class PaymentRecordCodes
        {
            /// <summary>
            /// Training Payment / 1
            /// </summary>
            public const int TrainingPayment = 1;

            /// <summary>
            /// Assessment Payment / 2
            /// </summary>
            public const int AssessmentPayment = 2;

            /// <summary>
            /// Employer payment reinmbursed by provider / 3
            /// </summary>
            public const int EmployerPaymentReimbursedByProvider = 3;

            /// <summary>
            /// Unassiged / 4
            /// </summary>
            public const int Unassigned4 = 4;

            /// <summary>
            /// Unassiged / 5
            /// </summary>
            public const int Unassigned5 = 5;

            /// <summary>
            /// Unassiged / 6
            /// </summary>
            public const int Unassigned6 = 6;

            /// <summary>
            /// Unassiged / 7
            /// </summary>
            public const int Unassigned7 = 7;

            /// <summary>
            /// Unassiged / 8
            /// </summary>
            public const int Unassigned8 = 8;

            /// <summary>
            /// Unassiged / 9
            /// </summary>
            public const int Unassigned9 = 9;

            /// <summary>
            /// Unassiged / 10
            /// </summary>
            public const int Unassigned10 = 10;
        }
    }
}
