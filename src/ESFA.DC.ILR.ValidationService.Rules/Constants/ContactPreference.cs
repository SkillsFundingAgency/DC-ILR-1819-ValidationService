namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// contact preferences (pre and post GDPR)
    /// </summary>
    public static class ContactPreference
    {
        /// <summary>
        /// no contact courses or opportunities (pre GDPR)
        /// </summary>
        public const string NoContactCoursesOrOpportunitiesPreGDPR = "RUI1";

        /// <summary>
        /// no contact surveys and research (pre GDPR)
        /// </summary>
        public const string NoContactSurveysAndResearchPreGDPR = "RUI2";

        /// <summary>
        /// no contact illness or died valid to 2013-07-31
        /// </summary>
        public const string NoContactIllnessOrDied_ValidTo20130731 = "RUI3";

        /// <summary>
        /// no contact due to illness
        /// </summary>
        public const string NoContactDueToIllness = "RUI4";

        /// <summary>
        /// no contact due to death
        /// </summary>
        public const string NoContactDueToDeath = "RUI5";

        /// <summary>
        /// agrees contact courses or opportunities (post GDPR)
        /// </summary>
        public const string AgreesContactCoursesOrOpportunitiesPostGDPR = "RUI6";

        /// <summary>
        /// agrees contact surveys and research (post GDPR)
        /// </summary>
        public const string AgreesContactSurveysAndResearchPostGDPR = "RUI7";

        /// <summary>
        /// no contact by post (pre GDPR)
        /// </summary>
        public const string NoContactByPostPreGDPR = "PMC1";

        /// <summary>
        /// no contact by phone (pre GDPR)
        /// </summary>
        public const string NoContactByPhonePreGDPR = "PMC2";

        /// <summary>
        /// no contact by email (pre GDPR)
        /// </summary>
        public const string NoContactByEmailPreGDPR = "PMC3";

        /// <summary>
        /// agrees contact by post (post GDPR)
        /// </summary>
        public const string AgreesContactByPostPostGDPR = "PMC4";

        /// <summary>
        /// agrees contact by phone (post GDPR)
        /// </summary>
        public const string AgreesContactByPhonePostGDPR = "PMC5";

        /// <summary>
        /// agrees contact by email (post GDPR)
        /// </summary>
        public const string AgreesContactByEmailPostGDPR = "PMC6";

        /// <summary>
        /// types of contact preference
        /// </summary>
        public static class Types
        {
            /// <summary>
            /// restricted user interaction
            /// </summary>
            public const string RestrictedUserInteraction = "RUI";

            /// <summary>
            /// The preferred method of contact
            /// </summary>
            public const string PreferredMethodOfContact = "PMC";
        }
    }
}
