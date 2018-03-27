using ESFA.DC.ILR.ValidationService.ExternalData.LARS.Model;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.ExternalData.Interface
{
    public interface IReferenceDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<long> UKPRNs { get; }
    }
}
