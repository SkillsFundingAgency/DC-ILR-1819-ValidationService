using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IReferenceDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<long> UKPRNs { get; }
    }
}
