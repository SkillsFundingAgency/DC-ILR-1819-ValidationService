using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class LearnerPerActorProviderService : ILearnerPerActorProviderService
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerPerActorProviderService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<IMessage>> ProvideAsync()
        {
            if (this._messageCache?.Item?.Learners == null)
            {
                return null;
            }

            var learners = _messageCache.Item.Learners.ToList();

            var learnersPerActors = CalculateLearnersPerActor(learners.Count);

            var learnerShards = learners.SplitList(learnersPerActors);

            // create IMessage shards with learners
            var messageShards = new List<IMessage>();
            var msg = _messageCache.Item as Message;
            foreach (var learnerShard in learnerShards)
            {
                var learnRefNumbers = learnerShard.Select(l => l.LearnRefNumber).ToCaseInsensitiveHashSet();

                // shallow duplication is sufficient except for the learners
                Message message = new Message();
                message.Header = msg.Header;
                message.LearnerDestinationandProgression = msg.LearnerDestinationandProgression?
                    .Where(ldp => learnRefNumbers.Contains(ldp.LearnRefNumber)).ToArray() ?? Array.Empty<MessageLearnerDestinationandProgression>();
                message.LearningProvider = msg.LearningProvider;
                message.SourceFiles = msg.SourceFiles;
                message.Learner = learnerShard.Cast<MessageLearner>().ToArray();
                messageShards.Add(message);
            }

            return messageShards;
        }

        public virtual int CalculateLearnersPerActor(int totalMessagesCount)
        {
            if (totalMessagesCount < 2000)
            {
                return 2000;
            }

            return 1000;
        }
    }
}
