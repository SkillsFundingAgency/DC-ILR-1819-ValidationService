namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule enterprise partnership
    /// </summary>
    public interface IEsfEligibilityRuleLocalEnterprisePartnership :
        IIdentifiableItem,
        IEsfEligibilityRuleSibling,
        IEsfEligibilityRuleReferences,
        IEsfEligibilityRuleCode<string>
    {
    }
}
