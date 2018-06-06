using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class LearnerProviderServiceTests
    {
        [Fact]
        public void Provide()
        {
            var learners = new List<TestLearner>();

            var testMessage = new TestMessage()
            {
                Learners = learners
            };

            var messageCacheMock = new Mock<ICache<IMessage>>();

            messageCacheMock.SetupGet(mc => mc.Item).Returns(testMessage);

            var learnerProviderService = new LearnerProviderService(messageCacheMock.Object);

            learnerProviderService.Provide().Should().BeSameAs(learners);
        }
    }
}
