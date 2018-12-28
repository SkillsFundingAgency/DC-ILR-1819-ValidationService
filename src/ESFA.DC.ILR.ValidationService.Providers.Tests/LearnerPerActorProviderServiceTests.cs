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
    public class LearnerPerActorProviderServiceTests
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
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerPerActorProviderService = new LearnerPerActorProviderService(messageCacheMock.Object);

            (await learnerPerActorProviderService.ProvideAsync()).Should().BeEquivalentTo(messages);
        }

        [Fact]
        public async Task ProvideAsync_LearnerDPMismatch()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner3" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerPerActorProviderService = new LearnerPerActorProviderService(messageCacheMock.Object);

            var lpa = await learnerPerActorProviderService.ProvideAsync();

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(1);
        }

        [Fact]
        public async Task ProvideAsync_LearnerDPNoMatch()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner3" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner4" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerPerActorProviderService = new LearnerPerActorProviderService(messageCacheMock.Object);

            var lpa = await learnerPerActorProviderService.ProvideAsync();

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(0);
        }

        [Fact]
        public async Task ProvideAsync_ZeroDPRecords()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerPerActorProviderService = new LearnerPerActorProviderService(messageCacheMock.Object);

            var lpa = await learnerPerActorProviderService.ProvideAsync();

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(0);
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
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);

            var learnerPerActorProviderServiceMock = new Mock<LearnerPerActorProviderService>(messageCacheMock.Object);
            learnerPerActorProviderServiceMock.Setup(ss => ss.CalculateLearnersPerActor(message.Learners.Count)).Returns(1);


            var lpa = (await learnerPerActorProviderServiceMock.Object.ProvideAsync()).ToArray();

            lpa.Select(m => m).Should().HaveCount(2);
            lpa[0].Learners.Should().HaveCount(1);
            lpa[1].Learners.Should().HaveCount(1);
        }
    }
}
