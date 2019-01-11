using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the framework common component implementation
    /// </summary>
    /// <seealso cref="ILARSFrameworkCommonComponent" />
    public class FrameworkCommonComponent :
        ILARSFrameworkCommonComponent
    {
        /*
        /// <summary>
        /// not applicable (shouldn't be here and apparently not used anyway...)
        /// </summary>
        public const int NotApplicable = -2; // ???
        */

        /// <summary>
        /// Gets or sets the framework work code.
        /// </summary>
        public int FworkCode { get; set; }

        /// <summary>
        /// Gets or sets the programme type.
        /// </summary>
        public int ProgType { get; set; }

        /// <summary>
        /// Gets or sets the pathway code.
        /// </summary>
        public int PwayCode { get; set; }

        /// <summary>
        /// Gets or sets the common component.
        /// </summary>
        public int CommonComponent { get; set; }

        /// <summary>
        /// Gets or sets the effective from (date).
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to (date).
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
    }
}
