using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ValidationService.Providers.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class MessageStringProviderServiceTests
    {
        [Fact]
        public void Provide()
        {
            var ilrString = "ILR String";
            var message = new Message();

            var stringProviderServiceMock = new Mock<IStringProviderService>();
            stringProviderServiceMock.Setup(sps => sps.Provide()).Returns(ilrString);

            var xmlSerializationService = new Mock<IXmlSerializationService>();
            xmlSerializationService.Setup(s => s.Deserialize<Message>(ilrString)).Returns(message);

            NewService(xmlSerializationService.Object, stringProviderServiceMock.Object).Provide().Should().BeSameAs(message);
        }

        private MessageStringProviderService NewService(IXmlSerializationService xmlSerializationService = null, IStringProviderService stringProviderService = null)
        {
            return new MessageStringProviderService(xmlSerializationService, stringProviderService);
        }
    }
}
