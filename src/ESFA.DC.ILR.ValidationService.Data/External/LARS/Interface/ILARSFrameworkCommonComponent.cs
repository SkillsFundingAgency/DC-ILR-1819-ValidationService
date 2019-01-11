using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars framework common component definition
    /// </summary>
    public interface ILARSFrameworkCommonComponent
    {
        /// <summary>
        /// Gets the framework work code.
        /// </summary>
        int FworkCode { get; }

        /// <summary>
        /// Gets the programme type.
        /// </summary>
        int ProgType { get; }

        /// <summary>
        /// Gets the pathway code.
        /// </summary>
        int PwayCode { get; }

        /// <summary>
        /// Gets the common component.
        /// </summary>
        int CommonComponent { get; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime? EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        DateTime? EffectiveTo { get; }
    }
}
