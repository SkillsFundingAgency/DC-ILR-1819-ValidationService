using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// lars validity
    /// </summary>
    public interface ILARSValidity :
        ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the learn aim reference.
        /// </summary>
        string LearnAimRef { get; }

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
