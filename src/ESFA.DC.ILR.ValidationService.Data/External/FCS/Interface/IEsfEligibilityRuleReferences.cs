namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF Eligibility Rule references definition
    /// </summary>
    public interface IEsfEligibilityRuleReferences
    {
        /// <summary>
        /// Gets the tender spec reference.
        /// </summary>
        string TenderSpecReference { get; }

        /// <summary>
        /// Gets the lot reference.
        /// </summary>
        string LotReference { get; }
    }
}
