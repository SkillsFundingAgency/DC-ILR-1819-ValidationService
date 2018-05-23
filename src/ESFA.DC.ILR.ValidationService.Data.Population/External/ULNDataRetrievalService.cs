using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ULNDataRetrievalService : AbstractDataRetrievalService, IULNDataRetrievalService
    {
        private readonly IULN _uln;

        public ULNDataRetrievalService(IULN uln, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _uln = uln;
        }

        public IEnumerable<long> Retrieve()
        {
            var uniqueULNs = UniqueULNsFromMessage(_messageCache.Item);

            return _uln.UniqueLearnerNumbers
                .Where(u => uniqueULNs.Contains(u.ULN))
                .Select(u => u.ULN);
        }

        public IEnumerable<long> UniqueULNsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Select(l => l.ULN)
                       .Distinct()
                   ?? new List<long>();
        }
    }
}
