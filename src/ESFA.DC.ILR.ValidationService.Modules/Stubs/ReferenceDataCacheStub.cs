using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.ExternalData.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Modules.Stubs
{
    public class ReferenceDataCacheStub : IReferenceDataCache
    {
        public ReferenceDataCacheStub()
        {
            ULNs = new List<long>();
            LearningDeliveries = new Dictionary<string, LearningDelivery>();
            UKPRNs = new List<long>();
        }

        public IReadOnlyCollection<long> ULNs { get; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        public IReadOnlyCollection<long> UKPRNs { get; }
    }
}
