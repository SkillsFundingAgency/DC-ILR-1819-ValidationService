using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ExternalDataCacheStub : IExternalDataCache
    {
        public ExternalDataCacheStub()
        {
            ULNs = new List<long>();
            LearningDeliveries = new Dictionary<string, LearningDelivery>();
            Frameworks = new List<Framework>();
            Organisations = new Dictionary<long, Organisation>();
            Postcodes = new List<string>();
        }

        public IReadOnlyCollection<long> ULNs { get; }

        public IReadOnlyDictionary<string, LearningDelivery> LearningDeliveries { get; }

        public IReadOnlyCollection<Framework> Frameworks { get; }

        public IReadOnlyDictionary<long, Organisation> Organisations { get; }

        public IReadOnlyCollection<string> Postcodes { get; }
    }
}
