using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars learning delivery
    /// </summary>
    /// <seealso cref="ILARSLearningDelivery" />
    public class LearningDelivery :
        ILARSLearningDelivery
    {
        /// <summary>
        /// Gets or sets the learn aim reference.
        /// </summary>
        public string LearnAimRef { get; set; }

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

        /// <summary>
        /// Gets or sets the learn aim reference type.
        /// </summary>
        public string LearnAimRefType { get; set; }

        /// <summary>
        /// Gets or sets the english prescribed Ids.
        /// </summary>
        public int? EnglPrscID { get; set; }

        /// <summary>
        /// Gets or sets the notional NVQ level.
        /// </summary>
        public string NotionalNVQLevel { get; set; }

        /// <summary>
        /// Gets or sets the notional NVQ level v2.
        /// </summary>
        public string NotionalNVQLevelv2 { get; set; }

        /// <summary>
        /// Gets or sets the framework common component.
        /// </summary>
        public int? FrameworkCommonComponent { get; set; }

        /// <summary>
        /// Gets or sets the learn direct class system code 1.
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode1 { get; set; }

        /// <summary>
        ///  Gets or sets the learn direct class system code 2
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode2 { get; set; }

        /// <summary>
        /// Gets or sets the learn direct class system code 3.
        /// </summary>
        public ILearnDirectClassSystemCode LearnDirectClassSystemCode3 { get; set; }

        /// <summary>
        ///  Gets or sets the Sector Subject Area Tier 1
        /// </summary>
        public decimal? SectorSubjectAreaTier1 { get; set; }

        /// <summary>
        ///  Gets or sets the Sector Subject Area Tier 2
        /// </summary>
        public decimal? SectorSubjectAreaTier2 { get; set; }

        /// <summary>
        /// Gets or sets the learning delivery categories.
        /// </summary>
        public IEnumerable<LearningDeliveryCategory> LearningDeliveryCategories { get; set; }

        /// <summary>
        /// Gets or sets the lars validities.
        /// </summary>
        public IEnumerable<LARSValidity> LARSValidities { get; set; }

        /// <summary>
        /// Gets or sets the framework aims.
        /// </summary>
        public IEnumerable<FrameworkAim> FrameworkAims { get; set; }

        /// <summary>
        /// Gets or sets the annual values.
        /// </summary>
        public IEnumerable<AnnualValue> AnnualValues { get; set; }

        /// <summary>
        /// Gets the learning delivery categories.
        /// </summary>
        IReadOnlyCollection<ILARSLearningCategory> ILARSLearningDelivery.LearningDeliveryCategories => LearningDeliveryCategories.AsSafeReadOnlyList();

        /// <summary>
        /// Gets the framework aims.
        /// </summary>
        IReadOnlyCollection<ILARSFrameworkAim> ILARSLearningDelivery.FrameworkAims => FrameworkAims.AsSafeReadOnlyList();

        /// <summary>
        /// Gets the annual values.
        /// </summary>
        IReadOnlyCollection<ILARSAnnualValue> ILARSLearningDelivery.AnnualValues => AnnualValues.AsSafeReadOnlyList();

        /// <summary>
        /// Gets the lars validities.
        /// </summary>
        IReadOnlyCollection<ILARSLearningDeliveryValidity> ILARSLearningDelivery.Validities => LARSValidities.AsSafeReadOnlyList();
    }
}
