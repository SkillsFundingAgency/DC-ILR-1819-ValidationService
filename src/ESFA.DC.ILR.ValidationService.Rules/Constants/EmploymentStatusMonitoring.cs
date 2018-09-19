namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of employment status
    /// </summary>
    public static class EmploymentStatusMonitoring
    {
        /// <summary>
        /// self employed
        /// </summary>
        public const string SelfEmployed = "SEI1";

        /// <summary>
        /// employed for 16 hours or more per week
        /// </summary>
        public const string EmployedFor16HoursOrMorePW = "EII1";

        /// <summary>
        /// employed for less than 16 hours per week
        /// </summary>
        public const string EmployedForLessThan16HoursPW = "EII2";

        /// <summary>
        /// employed for 16 to 19 hours per week
        /// </summary>
        public const string EmployedFor16To19HoursPW = "EII3";

        /// <summary>
        /// employed for 20 hours or more per week
        /// </summary>
        public const string EmployedFor20HoursOrMorePW = "EII4";

        /// <summary>
        /// employed for 0 to 10 hour per week
        /// </summary>
        public const string EmployedFor0To10HourPW = "EII5";

        /// <summary>
        /// employed for 11 to 20 hours per week
        /// </summary>
        public const string EmployedFor11To20HoursPW = "EII6";

        /// <summary>
        /// employed for 21 to 30 hours per week
        /// </summary>
        public const string EmployedFor21To30HoursPW = "EII7";

        /// <summary>
        /// employed for 31 plus hours per week
        /// </summary>
        public const string EmployedFor31PlusHoursPW = "EII8";

        /// <summary>
        /// unemployed for less than 6 months
        /// </summary>
        public const string UnemployedForLessThan6M = "LOU1";

        /// <summary>
        /// unemployed for 6 to 11 months
        /// </summary>
        public const string UnemployedFor6To11M = "LOU2";

        /// <summary>
        /// unemployed for 12 to 23 months
        /// </summary>
        public const string UnemployedFor12To23M = "LOU3";

        /// <summary>
        /// unemployed for 24 to 35 months
        /// </summary>
        public const string UnemployedFor24To35M = "LOU4";

        /// <summary>
        /// unemployed for 36 months and over
        /// </summary>
        public const string UnemployedFor36MPlus = "LOU5";

        /// <summary>
        /// employed for up to 3 months
        /// </summary>
        public const string EmployedForUpTo3M = "LOE1";

        /// <summary>
        /// employed for 4 to 6 months
        /// </summary>
        public const string EmployedFor4To6M = "LOE2";

        /// <summary>
        /// employed for 7 to 12 months
        /// </summary>
        public const string EmployedFor7To12M = "LOE3";

        /// <summary>
        /// employed for more than 12 months
        /// </summary>
        public const string EmployedForMoreThan12M = "LOE4";

        /// <summary>
        /// in receipt of job seekers allowance
        /// </summary>
        public const string InReceiptOfJobSeekersAllowance = "BSI1";

        /// <summary>
        /// in receipt of employment and support allowance
        /// </summary>
        public const string InReceiptOfEmploymentAndSupportAllowance = "BSI2";

        /// <summary>
        /// in receipt of another state benefit
        /// </summary>
        public const string InReceiptOfAnotherStateBenefit = "BSI3";

        /// <summary>
        /// in receipt of universal credit
        /// </summary>
        public const string InReceiptOfUniversalCredit = "BSI4";

        /// <summary>
        /// in fulltime education or training prior to enrolment
        /// </summary>
        public const string InFulltimeEducationOrTrainingPriorToEnrolment = "PEI1";

        /// <summary>
        /// small employer
        /// </summary>
        public const string SmallEmployer = "SEM1";

        /// <summary>
        /// types of employment status monitoring
        /// </summary>
        public static class Types
        {
            /// <summary>
            /// self employment indicator
            /// </summary>
            public const string SelfEmploymentIndicator = "SEI";

            /// <summary>
            /// employment intensity indicator
            /// </summary>
            public const string EmploymentIntensityIndicator = "EII";

            /// <summary>
            /// length of unemployment
            /// </summary>
            public const string LengthOfUnemployment = "LOU";

            /// <summary>
            /// length of employment
            /// </summary>
            public const string LengthOfEmployment = "LOE";

            /// <summary>
            /// benfit status indicator
            /// </summary>
            public const string BenfitStatusIndicator = "BSI";

            /// <summary>
            /// previous education indicator
            /// </summary>
            public const string PreviousEducationIndicator = "PEI";

            /// <summary>
            /// small employer
            /// </summary>
            public const string SmallEmployer = "SEM";
        }
    }
}