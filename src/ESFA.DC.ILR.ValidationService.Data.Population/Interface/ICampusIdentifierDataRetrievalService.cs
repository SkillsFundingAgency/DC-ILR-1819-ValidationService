using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ICampusIdentifierDataRetrievalService : IExternalDataRetrievalService<IReadOnlyCollection<ICampusIdentifier>>
    {
    }
}
