using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the ESF Eligibility Rule Local Authority implementation
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleLocalAuthority" />
    public class EsfEligibilityRuleLocalAuthority :
        IEsfEligibilityRuleLocalAuthority
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the tender spec reference.
        /// </summary>
        public string TenderSpecReference { get; set; }

        /// <summary>
        /// Gets or sets the lot reference.
        /// </summary>
        public string LotReference { get; set; }
    }
}
