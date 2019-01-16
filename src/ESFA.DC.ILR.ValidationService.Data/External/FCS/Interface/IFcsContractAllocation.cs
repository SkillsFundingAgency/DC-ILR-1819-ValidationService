using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS contract allocation definition
    /// </summary>
    public interface IFcsContractAllocation
    {
        /// <summary>
        /// Gets the tender spec reference.
        /// </summary>
        string TenderSpecReference { get; }

        /// <summary>
        /// Gets the lot reference.
        /// </summary>
        string LotReference { get; }

        /// <summary>
        /// Gets the contract allocation number.
        /// </summary>
        string ContractAllocationNumber { get; }

        /// <summary>
        /// Gets the funding stream period code.
        /// </summary>
        string FundingStreamPeriodCode { get; }

        IEsfEligibilityRule EsfEligibilityRule { get; }
    }
}
