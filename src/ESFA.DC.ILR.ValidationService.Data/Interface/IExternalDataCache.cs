using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IExternalDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<Framework> Frameworks { get; }

        IReadOnlyDictionary<long, Organisation> Organisations { get; }

        /// <summary>
        /// Gets the epa organisations.
        /// </summary>
        IReadOnlyCollection<IEPAOrganisation> EPAOrganisations { get; }

        IReadOnlyCollection<string> Postcodes { get; }

        IReadOnlyDictionary<string, ValidationError> ValidationErrors { get; }
    }
}
