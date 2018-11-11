using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the ESF Eligibility Rule Employment Status implementation
    /// </summary>
    /// <seealso cref="IEsfEligibilityRuleEmploymentStatus" />
    public class EsfEligibilityRuleEmploymentStatus :
        IEsfEligibilityRuleEmploymentStatus
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// 'add' me...
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the eligibility rule identifier.
        /// 'add' me...
        /// </summary>
        public int EligibilityRuleID { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the tender spec reference.
        /// </summary>
        public string TenderSpecReference { get; set; }

        /// <summary>
        /// Gets or sets the lot reference.
        /// </summary>
        public string LotReference { get; set; }

        /// <summary>
        /// Gets the esf eligibility rule.
        /// i need to be removed once the parent ID is available...
        /// </summary>
        public IEsfEligibilityRule EsfEligibilityRule => throw new NotSupportedException();
    }
}
