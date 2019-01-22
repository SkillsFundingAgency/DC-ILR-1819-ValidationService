namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// (types of) special fee indicators
    /// </summary>
    public class TypeOfSPECFEE
    {
        /// <summary>
        /// Standard/Prescribed fee
        /// </summary>
        public const int StandardPrescribedFee = 0;

        /// <summary>
        /// SandwichPlacement
        /// </summary>
        public const int SandwichPlacement = 1;

        /// <summary>
        /// Language year abroad and not full-year outgoing ERASMUS
        /// </summary>
        public const int LanguageYearAbroadAndNotFullYearOutgoingErasmus = 2;

        /// <summary>
        /// Full-year outgoing ERASMUS
        /// </summary>
        public const int FullYearOutgoingErasmus = 3;

        /// <summary>
        /// Final year of full-time course lasting less than 15 weeks
        /// </summary>
        public const int FinalYearFullTimeLessThan15Weeks = 4;

        /// <summary>
        /// Final year of a full-time course lasting more than 14 weeks but less than 24 weeks
        /// </summary>
        public const int FinalYearFullTimeBetween14And24Weeks = 5;

        /// <summary>
        /// Other Fee
        /// </summary>
        public const int OtherFee = 9;
    }
}
