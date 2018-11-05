using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class LearnerPerActorServiceStub : ILearnerPerActorService
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerPerActorServiceStub(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public IEnumerable<IMessage> Process()
        {
            var learners = _messageCache.Item.Learners.ToList();

            var learnersPerActors = CalculateLearnersPerActor(learners.Count);

            var learnerShards = SplitList(learners, learnersPerActors);

            // create IMessage shards with learners
            var messageShards = new List<IMessage>();
            var msg = _messageCache.Item as Message;
            foreach (var learnerShard in learnerShards)
            {
                // shallow duplication is sufficient except for the learners
                Message message = new Message();
                message.Header = msg.Header;
                message.LearnerDestinationandProgression = msg.LearnerDestinationandProgression;
                message.LearningProvider = msg.LearningProvider;
                message.SourceFiles = msg.SourceFiles;
                message.Learner = learnerShard.Cast<MessageLearner>().ToArray();
                messageShards.Add(message);
            }

            return messageShards;
        }

        private int CalculateLearnersPerActor(int totalMessagesCount)
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

        private IEnumerable<IEnumerable<ILearner>> SplitList(IEnumerable<ILearner> learners, int nSize = 30)
        {
            var learnerList = learners.ToList();

            for (var i = 0; i < learnerList.Count; i += nSize)
            {
                yield return learnerList.GetRange(i, Math.Min(nSize, learnerList.Count - i));
            }
        }
    }
}
