namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule local authority definition
    /// </summary>
    public interface IEsfEligibilityRuleLocalAuthority : IEsfEligibilityRuleReferences
    {
        string Code { get; }
    }
}
