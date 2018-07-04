using System;
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

            List<long> result = new List<long>(uniqueULNs.Count());
            var ulnShards = SplitList(uniqueULNs, 5000);
            foreach (var shard in ulnShards)
            {
                result.AddRange(_uln.UniqueLearnerNumbers
                .Where(u => shard.Contains(u.ULN))
                .Select(u => u.ULN));
            }

            return result;
        }

        public IEnumerable<long> UniqueULNsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Select(l => l.ULN)
                       .Distinct()
                   ?? new List<long>();
        }

        private IEnumerable<IEnumerable<long>> SplitList(IEnumerable<long> ulns, int nSize = 30)
        {
            var ulnList = ulns.ToList();

            for (var i = 0; i < ulnList.Count; i += nSize)
            {
                yield return ulnList.GetRange(i, Math.Min(nSize, ulnList.Count - i));
            }
        }
    }
}
