namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF Eligibility Rule sibling definition
    /// </summary>
    public interface IEsfEligibilityRuleSibling
    {
        /// <summary>
        /// Gets the eligibility rule identifier.
        /// 'add' me...
        /// </summary>
        int EligibilityRuleID { get; }

        /// <summary>
        /// Gets the esf eligibility rule.
        /// remove me..
        /// </summary>
        IEsfEligibilityRule EsfEligibilityRule { get; }
    }
}
