using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IExternalDataCache
    {
        IReadOnlyCollection<long> ULNs { get; }

        IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        IReadOnlyCollection<Framework> Frameworks { get; }

        IReadOnlyCollection<long> UKPRNs { get; }
    }
}
