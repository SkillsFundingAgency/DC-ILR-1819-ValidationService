using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the ESF Eligibility Rule Code definition
    /// </summary>
    /// <typeparam name="TCode">The type of the code.</typeparam>
    public interface IEsfEligibilityRuleCode<TCode>
        where TCode : IComparable, IConvertible
    {
        TCode Code { get; }
    }
}
