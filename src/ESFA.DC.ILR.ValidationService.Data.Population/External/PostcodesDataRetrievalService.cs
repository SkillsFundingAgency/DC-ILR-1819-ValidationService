using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class PostcodesDataRetrievalService : AbstractDataRetrievalService, IPostcodesDataRetrievalService
    {
        private readonly IPostcodes _postcodes;

        public PostcodesDataRetrievalService()
            : base(null)
        {
        }

        public PostcodesDataRetrievalService(IPostcodes postcodes, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _postcodes = postcodes;
        }

        public async Task<IEnumerable<string>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var uniquePostcodes = UniquePostcodesFromMessage(_messageCache.Item).ToList();

            return await _postcodes.MasterPostcodes
                .Where(p => uniquePostcodes.Contains(p.Postcode))
                .Select(p => p.Postcode)
                .ToListAsync(cancellationToken);
        }

        public IEnumerable<string> UniquePostcodesFromMessage(IMessage message)
        {
            return UniqueLearnerPostcodesFromMessage(message)
                .Union(UniqueLearnerPostcodePriorsFromMessage(message))
                .Union(UniqueLearningDeliveryLocationPostcodesFromMessage(message))
                .Distinct();
        }

        public virtual IEnumerable<string> UniqueLearnerPostcodesFromMessage(IMessage message)
        {
            return message?
                        .Learners?
                        .Where(l => l.Postcode != null)
                        .Select(l => l.Postcode)
                        .Distinct()
                    ?? new List<string>();
        }

        public virtual IEnumerable<string> UniqueLearnerPostcodePriorsFromMessage(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.PostcodePrior != null)
                       .Select(l => l.PostcodePrior)
                       .Distinct()
                   ?? new List<string>();
        }

        public virtual IEnumerable<string> UniqueLearningDeliveryLocationPostcodesFromMessage(IMessage message)
        {
            return message?
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Select(ld => ld.DelLocPostCode)
                       .Distinct()
                   ?? new List<string>();
        }
    }
}
