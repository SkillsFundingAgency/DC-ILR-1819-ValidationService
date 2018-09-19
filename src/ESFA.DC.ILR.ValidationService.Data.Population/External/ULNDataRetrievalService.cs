using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<long>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var uniqueULNs = UniqueULNsFromMessage(_messageCache.Item);

            List<long> result = new List<long>(uniqueULNs.Count());
            var ulnShards = SplitList(uniqueULNs, 5000);
            foreach (var shard in ulnShards)
            {
                result.AddRange(await _uln.UniqueLearnerNumbers
                .Where(u => shard.Contains(u.ULN))
                .Select(u => u.ULN)
                .ToListAsync(cancellationToken));
            }

            return result;
        }

        public IEnumerable<long> UniqueULNsFromMessage(IMessage message)
        {
            return message?
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
