using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the ESF Eligibility Rule Employment Status implementation
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleEmploymentStatus" />
    public class EsfEligibilityRuleEmploymentStatus : IEsfEligibilityRuleEmploymentStatus
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public int Code { get; set; }

        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }
    }
}
