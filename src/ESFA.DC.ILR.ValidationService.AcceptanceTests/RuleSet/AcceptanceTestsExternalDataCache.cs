using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AcceptanceTestsExternalDataCache : IExternalDataCache
    {
        public AcceptanceTestsExternalDataCache()
        {
        }

        public IReadOnlyCollection<long> ULNs { get; set; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; set; }

        public IReadOnlyCollection<long> UKPRNs { get; set; }

        public IReadOnlyCollection<Framework> Frameworks { get; set; }
    }
}
