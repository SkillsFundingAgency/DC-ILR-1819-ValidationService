using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars framework aim class
    /// </summary>
    /// <seealso cref="ILARSFrameworkAim" />
    public class FrameworkAim :
        ILARSFrameworkAim
    {
        /// <summary>
        /// Gets or sets the framework code.
        /// </summary>
        public int FworkCode { get; set; }

        /// <summary>
        /// Gets or sets the type of the programme.
        /// </summary>
        public int ProgType { get; set; }

        /// <summary>
        /// Gets or sets the pathway code.
        /// </summary>
        public int PwayCode { get; set; }

        /// <summary>
        /// Gets or sets the learning aim reference.
        /// </summary>
        public string LearnAimRef { get; set; }

        /// <summary>
        /// Gets or sets the type of the framework component.
        /// </summary>
        public int? FrameworkComponentType { get; set; }

        /// <summary>
        /// Gets or sets the effective from date.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to date.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        public DateTime StartDate => EffectiveFrom;

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        public DateTime? EndDate => EffectiveTo;
    }
}
