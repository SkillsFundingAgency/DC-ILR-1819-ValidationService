using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IFCSDataRetrievalService :
        IExternalDataRetrievalService<IReadOnlyDictionary<string, IFcsContractAllocation>>
    {
    }
}
