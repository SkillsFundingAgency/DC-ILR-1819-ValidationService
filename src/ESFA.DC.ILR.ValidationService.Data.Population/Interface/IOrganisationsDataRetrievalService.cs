using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IOrganisationsDataRetrievalService : IExternalDataRetrievalService<IReadOnlyDictionary<long, Organisation>>
    {
    }
}
