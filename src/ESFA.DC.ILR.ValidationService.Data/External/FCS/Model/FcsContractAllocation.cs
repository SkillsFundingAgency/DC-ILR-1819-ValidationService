﻿using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the FCS Contract Allocation implementation
    /// </summary>
    /// <seealso cref="IFcsContractAllocation" />
    public class FcsContractAllocation : IFcsContractAllocation
    {
        /// <summary>
        /// Gets or sets the contract allocation number.
        /// </summary>
        public string ContractAllocationNumber { get; set; }

        /// <summary>
        /// Gets or sets the tender spec reference.
        /// </summary>
        public string TenderSpecReference { get; set; }

        /// <summary>
        /// Gets or sets the lot reference.
        /// </summary>
        public string LotReference { get; set; }

        /// <summary>
        /// Gets or sets the funding stream period code.
        /// </summary>
        public string FundingStreamPeriodCode { get; set; }

        public IEsfEligibilityRule EsfEligibilityRule { get; set; }
    }
}
