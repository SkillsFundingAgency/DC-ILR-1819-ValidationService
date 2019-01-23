using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars standard class
    /// </summary>
    /// <seealso cref="ILARSStandard" />
    public class LARSStandard : ILARSStandard
    {
        /// <summary>
        /// Gets or sets the standard code.
        /// </summary>
        public int StandardCode { get; set; }

        /// <summary>
        /// Gets or sets the standard sector code.
        /// </summary>
        public string StandardSectorCode { get; set; }

        /// <summary>
        /// Gets or sets the notional end level.
        /// </summary>
        public string NotionalEndLevel { get; set; }

        /// <summary>
        /// Gets or sets the effective from date.
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to date.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        public IEnumerable<ILARSStandardFunding> StandardsFunding { get; set; }
    }
}
