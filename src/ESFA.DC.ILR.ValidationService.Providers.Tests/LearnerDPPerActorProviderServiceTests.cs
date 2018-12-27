using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class LearnerDPPerActorProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerDPPerActorProviderServiceMock = new LearnerDPPerActorProviderService(messageCacheMock.Object);

            (await learnerDPPerActorProviderServiceMock.ProvideAsync()).Should().BeEquivalentTo(messages);
        }

        [Fact]
        public async Task ProvideAsync_MultipleShards()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerDPPerActorProviderServiceMock = new Mock<LearnerDPPerActorProviderService>(messageCacheMock.Object);
            learnerDPPerActorProviderServiceMock.Setup(ss => ss.CalculateLearnerDPsPerActor(message.LearnerDestinationAndProgressions.Count)).Returns(1);


            var lpa = (await learnerDPPerActorProviderServiceMock.Object.ProvideAsync()).ToArray();

            lpa.Select(m => m).Should().HaveCount(2);
            lpa[0].Learners.Should().BeNullOrEmpty();
            lpa[1].Learners.Should().BeNullOrEmpty();
            lpa[0].LearnerDestinationAndProgressions.Should().HaveCount(1);
            lpa[1].LearnerDestinationAndProgressions.Should().HaveCount(1);
        }
    }
}
