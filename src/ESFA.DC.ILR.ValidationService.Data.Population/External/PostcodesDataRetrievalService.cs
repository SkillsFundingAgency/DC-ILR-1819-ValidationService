using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class PostcodesDataRetrievalService : IExternalDataRetrievalService<IEnumerable<string>, IMessage>
    {
        private readonly IPostcodes _postcodes;

        public PostcodesDataRetrievalService(IPostcodes postcodes)
        {
            _postcodes = postcodes;
        }

        public IEnumerable<string> Retrieve(IMessage message)
        {
            var uniquePostcodes = UniquePostcodesFromMessage(message).ToList();

            return _postcodes.MasterPostcodes
                .Where(p => uniquePostcodes.Contains(p.Postcode))
                .Select(p => p.Postcode);
        }

        public IEnumerable<string> UniquePostcodesFromMessage(IMessage message)
        {
            var learnerPostcodes = message
                                       .Learners?
                                       .Where(l => l.Postcode != null)
                                       .Select(l => l.Postcode)
                                       .Distinct()
                                   ?? new List<string>();

            var learnerPostcodePriors = message
                                            .Learners?
                                            .Where(l => l.PostcodePrior != null)
                                            .Select(l => l.PostcodePrior)
                                            .Distinct()
                                        ?? new List<string>();

            var learningDeliveryLocationPostcodes = message
                                                        .Learners?
                                                        .Where(l => l.LearningDeliveries != null)
                                                        .SelectMany(l => l.LearningDeliveries)
                                                        .Select(ld => ld.DelLocPostCode)
                                                        .Distinct()
                                                    ?? new List<string>();

            return learnerPostcodes
                .Union(learnerPostcodePriors)
                .Union(learningDeliveryLocationPostcodes)
                .Distinct();
        }
    }
}
