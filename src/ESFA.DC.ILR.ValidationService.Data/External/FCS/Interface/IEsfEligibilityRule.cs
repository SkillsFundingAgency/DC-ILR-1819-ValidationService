namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF eligibility rule definition
    /// </summary>
    public interface IEsfEligibilityRule :
        IIdentifiableItem,
        IEsfEligibilityRuleReferences
    {
        /// <summary>
        /// Gets the minimum age.
        /// </summary>
        int? MinAge { get; }

        /// <summary>
        /// Gets the maximum age.
        /// </summary>
        int? MaxAge { get; }

        /// <summary>
        /// Gets the minimum length of unemployment.
        /// </summary>
        int? MinLengthOfUnemployment { get; }

        /// <summary>
        /// Gets the maximum length of unemployment.
        /// </summary>
        int? MaxLengthOfUnemployment { get; }

        /// <summary>
        /// Gets the minimum prior attainment.
        /// </summary>
        string MinPriorAttainment { get; }

        /// <summary>
        /// Gets the maximum prior attainment.
        /// </summary>
        string MaxPriorAttainment { get; }

        /// <summary>
        /// Gets the benefits.
        /// </summary>
        bool? Benefits { get; }

        /// <summary>
        /// Gets the calculate method.
        /// </summary>
        int? CalcMethod { get; }

        // lists not required here...
        // IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> EmploymentStatuses { get; }
        // IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> LocalAuthorities { get; }
        // IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> LocalEnterprisePartnerships { get; }
    }
}
