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
    public class LearnerDPPerActorProviderService : ILearnerDPPerActorProviderService
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerDPPerActorProviderService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<IMessage>> ProvideAsync()
        {
            if (this._messageCache?.Item?.LearnerDestinationAndProgressions == null)
            {
                return null;
            }

            var learnerDPs = _messageCache.Item.LearnerDestinationAndProgressions.ToList();

            var learnerDPsPerActors = CalculateLearnerDPsPerActor(learnerDPs.Count);

            var learnerDPShards = learnerDPs.SplitList(learnerDPsPerActors);

            // create IMessage shards with learners
            var messageShards = new List<IMessage>();
            var msg = _messageCache.Item as Message;
            foreach (var learnerDPShard in learnerDPShards)
            {
                // shallow duplication is sufficient except for the learners
                Message message = new Message();
                message.Header = msg.Header;
                message.LearnerDestinationandProgression = learnerDPShard.Cast<MessageLearnerDestinationandProgression>().ToArray();
                message.LearningProvider = msg.LearningProvider;
                message.SourceFiles = msg.SourceFiles;
                messageShards.Add(message);
            }

            return messageShards;
        }

        public virtual int CalculateLearnerDPsPerActor(int totalMessagesCount)
        {
            if (totalMessagesCount < 2000)
            {
                return 2000;
            }

            return 1000;

            if (totalMessagesCount <= 1700)
            {
                return 500;
            }

            if (totalMessagesCount <= 10000)
            {
                return 1000;
            }

            if (totalMessagesCount <= 30000)
            {
                return 5000;
            }

            return 10000;
        }
    }
}
