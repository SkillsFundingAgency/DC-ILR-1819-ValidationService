using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    /// <summary>
    /// the LARS standard validity data retrival service
    /// </summary>
    /// <seealso cref="IExternalDataRetrievalService{List{LARSStandardValidity}}" />
    public interface ILARSStandardValidityDataRetrievalService :
        IExternalDataRetrievalService<List<LARSStandardValidity>>
    {
    }
}
