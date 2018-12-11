namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF Eligibility Rule Employment Status definition
    /// </summary>
    public interface IEsfEligibilityRuleEmploymentStatus :
        IIdentifiableItem,
        IEsfEligibilityRuleSibling,
        IEsfEligibilityRuleReferences,
        IEsfEligibilityRuleCode<int>
    {
    }
}
