using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class LearnerDPProviderServiceTests
    {
        [Fact]
        public async Task Provide()
        {
            var learnerDPs = new List<TestLearnerDestinationAndProgression>();

            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = learnerDPs
            };


            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(testMessage);

            var learnerDPProviderService = new LearnerDPProviderService(messageCacheMock.Object);

            (await learnerDPProviderService.ProvideAsync(CancellationToken.None)).Should().BeSameAs(learnerDPs);
        }
    }
}
