﻿using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    /// <summary>
    /// the lars framework type implementation
    /// </summary>
    public class Framework :
        ILARSFramework
    {
        /// <summary>
        /// The framework aims
        /// </summary>
        private IEnumerable<ILARSFrameworkAim> _frameworkAims;

        /// <summary>
        /// The framework common components
        /// </summary>
        private IEnumerable<ILARSFrameworkCommonComponent> _frameworkCommonComponents;

        /// <summary>
        /// Gets or sets the framework code.
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
        /// Gets or sets the effective from (date).
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to (date).
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Gets or sets the framework aims.
        /// </summary>
        public IEnumerable<ILARSFrameworkAim> FrameworkAims
        {
            get => _frameworkAims ?? (_frameworkAims = Collection.EmptyAndReadOnly<ILARSFrameworkAim>());
            set => _frameworkAims = value;
        }

        /// <summary>
        /// Gets or sets the framework common components.
        /// </summary>
        public IEnumerable<ILARSFrameworkCommonComponent> FrameworkCommonComponents
        {
            get => _frameworkCommonComponents ?? (_frameworkCommonComponents = Collection.EmptyAndReadOnly<ILARSFrameworkCommonComponent>());
            set => _frameworkCommonComponents = value;
        }
    }
}
