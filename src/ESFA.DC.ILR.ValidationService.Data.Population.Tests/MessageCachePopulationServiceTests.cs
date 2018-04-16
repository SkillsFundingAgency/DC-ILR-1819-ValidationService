using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class MessageCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var message = new Mock<IMessage>().Object;

            var messageCacheMock = new Mock<Cache<IMessage>>();

            messageCacheMock.SetupSet(mc => mc.Item = message).Verifiable();

            var messageValidationItemProviderServiceMock = new Mock<IValidationItemProviderService<IMessage>>();

            messageValidationItemProviderServiceMock.Setup(ps => ps.Provide()).Returns(message);

            NewService(messageCacheMock.Object, messageValidationItemProviderServiceMock.Object).Populate();

            messageValidationItemProviderServiceMock.Verify();
            messageCacheMock.Verify();
        }

        private MessageCachePopulationService NewService(ICache<IMessage> messageCache = null, IValidationItemProviderService<IMessage> messageValidationItemProviderService = null)
        {
            return new MessageCachePopulationService(messageCache, messageValidationItemProviderService);
        }
    }
}
