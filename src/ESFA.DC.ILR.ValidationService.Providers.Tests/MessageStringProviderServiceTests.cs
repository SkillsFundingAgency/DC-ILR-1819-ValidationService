﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class MessageStringProviderServiceTests
    {
        [Fact]
        public async Task Provide()
        {
            var cancellationToken = CancellationToken.None;

            var ilrString = "ILR String";
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(ilrString));
            var message = new Message();

            var stringProviderServiceMock = new Mock<IMessageStreamProviderService>();
            stringProviderServiceMock.Setup(sps => sps.Provide(cancellationToken)).ReturnsAsync(memoryStream);
            
            var xmlSerializationService = new Mock<IXmlSerializationService>();
            xmlSerializationService.Setup(s => s.Deserialize<Message>(memoryStream)).Returns(message);

            (await NewService(xmlSerializationService.Object, stringProviderServiceMock.Object).ProvideAsync(cancellationToken)).Should().BeSameAs(message);
        }

        private MessageFileProviderService NewService(IXmlSerializationService xmlSerializationService = null, IMessageStreamProviderService stringProviderService = null)
        {
            return new MessageFileProviderService(xmlSerializationService, stringProviderService);
        }
    }
}
