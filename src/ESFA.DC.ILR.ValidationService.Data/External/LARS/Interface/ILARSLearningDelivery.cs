using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// the lars learning delivery
    /// </summary>
    public interface ILARSLearningDelivery
    {
        /// <summary>
        /// Gets the learn aim reference.
        /// </summary>
        string LearnAimRef { get; }

        /// <summary>
        /// Gets the effective from date.
        /// </summary>
        DateTime EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to date.
        /// </summary>
        DateTime? EffectiveTo { get; }

        /// <summary>
        /// Gets the learn aim reference type.
        /// </summary>
        string LearnAimRefType { get; }

        /// <summary>
        /// Gets the english prescribed Ids.
        /// </summary>
        int? EnglPrscID { get; }

        /// <summary>
        /// Gets the notional NVQ level.
        /// </summary>
        string NotionalNVQLevel { get; }

        /// <summary>
        /// Gets the notional NVQ level v2.
        /// </summary>
        string NotionalNVQLevelv2 { get; }

        /// <summary>
        /// Gets the framework common component.
        /// </summary>
        int? FrameworkCommonComponent { get; }

        /// <summary>
        /// Gets the learn direct class system code 1.
        /// </summary>
        ILearnDirectClassSystemCode LearnDirectClassSystemCode1 { get; }

        /// <summary>
        /// Gets the learn direct class system code 2
        /// </summary>
        ILearnDirectClassSystemCode LearnDirectClassSystemCode2 { get; }

        /// <summary>
        /// Gets the learn direct class system code 3.
        /// </summary>
        ILearnDirectClassSystemCode LearnDirectClassSystemCode3 { get; }

        /// <summary>
        ///  Gets the Sector Subject Area Tier 1
        /// </summary>
        decimal? SectorSubjectAreaTier1 { get; }

        /// <summary>
        ///  Gets the Sector Subject Area Tier 2
        /// </summary>
        decimal? SectorSubjectAreaTier2 { get; }

        /// <summary>
        /// Gets the learning delivery categories.
        /// </summary>
        IReadOnlyCollection<ILARSLearningCategory> LearningDeliveryCategories { get; }

        /// <summary>
        /// Gets the framework aims.
        /// </summary>
        IReadOnlyCollection<ILARSFrameworkAim> FrameworkAims { get; }

        /// <summary>
        /// Gets the annual values.
        /// </summary>
        IReadOnlyCollection<ILARSAnnualValue> AnnualValues { get; }

        /// <summary>
        /// Gets the lars validities.
        /// </summary>
        IReadOnlyCollection<ILARSValidity> LARSValidities { get; }
    }
}
