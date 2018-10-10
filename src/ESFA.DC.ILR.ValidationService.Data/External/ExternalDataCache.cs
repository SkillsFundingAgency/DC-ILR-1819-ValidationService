using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External
{
    public class ExternalDataCache : IExternalDataCache
    {
        public IReadOnlyCollection<long> ULNs { get; set; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; set; }

        public IReadOnlyCollection<Framework> Frameworks { get; set; }

        public IReadOnlyDictionary<long, Organisation.Model.Organisation> Organisations { get; set; }

        /// <summary>
        /// Gets or sets the LARS standard validities.
        /// </summary>
        public IReadOnlyCollection<LARSStandardValidity> StandardValidities { get; set; }

        /// <summary>
        /// Gets or sets the epa organisations.
        /// </summary>
        public IReadOnlyCollection<IEPAOrganisation> EPAOrganisations { get; set; }

        public IReadOnlyCollection<string> Postcodes { get; set; }

        public IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; set; }
    }
}
