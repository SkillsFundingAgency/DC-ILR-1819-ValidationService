using System.Threading;
using System.Threading.Tasks;
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
        public async Task Populate()
        {
            var message = new Mock<IMessage>().Object;

            var messageCacheMock = new Mock<Cache<IMessage>>();

            messageCacheMock.SetupSet(mc => mc.Item = message).Verifiable();

            var messageValidationItemProviderServiceMock = new Mock<IValidationItemProviderService<IMessage>>();

            messageValidationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(message);

            await NewService(messageCacheMock.Object, messageValidationItemProviderServiceMock.Object).PopulateAsync(CancellationToken.None);

            messageValidationItemProviderServiceMock.Verify();
            messageCacheMock.Verify();
        }

        private MessageCachePopulationService NewService(ICache<IMessage> messageCache = null, IValidationItemProviderService<IMessage> messageValidationItemProviderService = null)
        {
            return new MessageCachePopulationService(messageCache, messageValidationItemProviderService);
        }
    }
}
