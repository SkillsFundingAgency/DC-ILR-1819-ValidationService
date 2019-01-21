﻿namespace ESFA.DC.ILR.ValidationService.Data.External
{
    using System.Collections.Generic;
    using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
    using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
    using ESFA.DC.ILR.ValidationService.Data.Interface;

    /// <summary>
    /// The external data cache implementation,
    /// model to be reflected within the validation actor.
    /// </summary>
    /// <seealso cref="IExternalDataCache" />
    public class ExternalDataCache :
        IExternalDataCache
    {
        public IReadOnlyCollection<long> ULNs { get; set; }

        /// <summary>
        /// Gets or sets the employer reference numbers.
        /// </summary>
        public IReadOnlyCollection<int> ERNs { get; set; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; set; }

        public IReadOnlyCollection<Framework> Frameworks { get; set; }

        public IReadOnlyDictionary<long, Organisation.Model.Organisation> Organisations { get; set; }

        /// <summary>
        /// Gets or sets the LARS standards.
        /// </summary>
        public IReadOnlyCollection<ILARSStandard> Standards { get; set; }

        /// <summary>
        /// Gets or sets the LARS standard validities.
        /// </summary>
        public IReadOnlyCollection<ILARSStandardValidity> StandardValidities { get; set; }

        /// <summary>
        /// Gets or sets the epa organisations.
        /// </summary>
        public IReadOnlyCollection<IEPAOrganisation> EPAOrganisations { get; set; }

        public IReadOnlyCollection<string> Postcodes { get; set; }

        /// <summary>
        /// Gets or sets the ons postcodes.
        /// </summary>
        public IReadOnlyCollection<IONSPostcode> ONSPostcodes { get; set; }

        public IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; set; }

        /// <summary>
        /// Gets or sets the FCS contract allocations.
        /// </summary>
        public IReadOnlyDictionary<string, IFcsContractAllocation> FCSContractAllocations { get; set; }
    }
}
