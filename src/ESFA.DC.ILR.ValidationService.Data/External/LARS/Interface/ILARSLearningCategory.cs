using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars learning category
    /// </summary>
    public interface ILARSLearningCategory
    {
        /// <summary>
        /// Gets the learn aim reference.
        /// </summary>
        string LearnAimRef { get; }

        /// <summary>
        /// Gets the category reference.
        /// </summary>
        int CategoryRef { get; }

        /// <summary>
        /// Gets the effective from date.
        /// </summary>
        DateTime EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to date.
        /// </summary>
        DateTime? EffectiveTo { get; }
    }
}
