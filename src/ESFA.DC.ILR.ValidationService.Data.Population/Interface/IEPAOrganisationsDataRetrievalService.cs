using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IEPAOrganisationsDataRetrievalService :
        IExternalDataRetrievalService<IReadOnlyDictionary<string, List<EPAOrganisations>>>
    {
    }
}
