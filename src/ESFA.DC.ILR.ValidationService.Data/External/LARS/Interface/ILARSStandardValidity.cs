using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// lars validity
    /// </summary>
    public interface ILARSStandardValidity :
        ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the standard code.
        /// </summary>
        int StandardCode { get; }

        /// <summary>
        /// Gets the validity category.
        /// </summary>
        string ValidityCategory { get; }

        /// <summary>
        /// Gets the last new start date.
        /// </summary>
        DateTime? LastNewStartDate { get; }
    }
}
