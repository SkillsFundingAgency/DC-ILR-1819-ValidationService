using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars learning category
    /// </summary>
    /// <seealso cref="ILARSLearningCategory" />
    public sealed class LearningDeliveryCategory :
        ILARSLearningCategory
    {
        /// <summary>
        /// Gets or sets the learn aim reference.
        /// </summary>
        public string LearnAimRef { get; set; }

        /// <summary>
        /// Gets or sets the category reference.
        /// </summary>
        public int CategoryRef { get; set; }

        /// <summary>
        /// Gets or sets the effective from date.
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to date.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
    }
}
