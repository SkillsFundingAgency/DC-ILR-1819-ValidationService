using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IFCSDataRetrievalService : IExternalDataRetrievalService<IReadOnlyCollection<FcsContract>>
    {
    }
}
