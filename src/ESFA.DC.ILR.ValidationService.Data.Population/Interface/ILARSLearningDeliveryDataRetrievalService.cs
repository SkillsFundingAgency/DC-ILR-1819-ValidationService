using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ILARSLearningDeliveryDataRetrievalService : IExternalDataRetrievalService<IReadOnlyDictionary<string, LearningDelivery>>
    {
    }
}
