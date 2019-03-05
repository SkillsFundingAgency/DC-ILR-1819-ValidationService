using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars standard validity class
    /// </summary>
    /// <seealso cref="ILARSStandardValidity" />
    public class LARSStandardValidity :
        ILARSStandardValidity
    {
        /// <summary>
        /// Gets or sets the standard code.
        /// </summary>
        public int StandardCode { get; set; }

        /// <summary>
        /// Gets or sets the validity category.
        /// </summary>
        public string ValidityCategory { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the last new start date.
        /// </summary>
        public DateTime? LastNewStartDate { get; set; }
    }
}
