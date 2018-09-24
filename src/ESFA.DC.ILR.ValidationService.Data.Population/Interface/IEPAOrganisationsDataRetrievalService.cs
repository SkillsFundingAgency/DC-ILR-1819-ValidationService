using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IEPAOrganisationsDataRetrievalService :
        IExternalDataRetrievalService<IReadOnlyCollection<IEPAOrganisation>>
    {
    }
}
