using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public sealed class AzureStorageCompressedFileContentStringProviderServiceTests
    {
        [Fact]
        public async Task ValidTest()
        {
            const string file = @"Files\ILR_Valid.zip";
            Mock<ILogger> logger = new Mock<ILogger>();

            IPreValidationContext preValidationContext = new PreValidationContext
            {
                Input = file,
                JobId = "1"
            };
            Mock<IStreamableKeyValuePersistenceService> streamableKeyValuePersistenceService = new Mock<IStreamableKeyValuePersistenceService>();
            streamableKeyValuePersistenceService
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((str, s, c) => File.Open(file, FileMode.Open).CopyTo(s))
                .Returns(Task.CompletedTask);
            streamableKeyValuePersistenceService
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            Mock<IValidationErrorHandler> validationErrorHandler = new Mock<IValidationErrorHandler>();
            Mock<IDateTimeProvider> dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

            IMessageStreamProviderService messageStreamProviderService =
                new AzureStorageCompressedFileContentStringProviderService(
                    preValidationContext,
                    logger.Object,
                    streamableKeyValuePersistenceService.Object,
                    validationErrorHandler.Object,
                    dateTimeProvider.Object);
            Stream stream = await messageStreamProviderService.Provide(CancellationToken.None);

            Assert.NotNull(stream);
        }

        [Theory]
        [InlineData(@"Files\Zip_In_Zip.zip", "ZIP_EMPTY")]
        [InlineData(@"Files\Invalid_Zip.zip", "ZIP_CORRUPT")]
        [InlineData(@"Files\Zip_Too_Many_Files.zip", "ZIP_TOO_MANY_FILES")]
        public async Task ZipTest(string file, string expectedRule)
        {
            string foundRule = string.Empty;
            Mock<ILogger> logger = new Mock<ILogger>();

            IPreValidationContext preValidationContext = new PreValidationContext
            {
                Input = file,
                JobId = "1"
            };
            Mock<IStreamableKeyValuePersistenceService> streamableKeyValuePersistenceService = new Mock<IStreamableKeyValuePersistenceService>();
            streamableKeyValuePersistenceService
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<string, Stream, CancellationToken>((str, s, c) => File.Open(file, FileMode.Open).CopyTo(s))
                .Returns(Task.CompletedTask);
            streamableKeyValuePersistenceService
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            Mock<IValidationErrorHandler> validationErrorHandler = new Mock<IValidationErrorHandler>();
            validationErrorHandler.Setup(x => x.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()))
                .Callback<string, string, long?, IEnumerable< IErrorMessageParameter>>((ruleName, learnRef, aimSeqNum, parms) => foundRule = ruleName)
                .Verifiable();
            Mock<IDateTimeProvider> dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

            IMessageStreamProviderService messageStreamProviderService =
                new AzureStorageCompressedFileContentStringProviderService(
                    preValidationContext,
                    logger.Object,
                    streamableKeyValuePersistenceService.Object,
                    validationErrorHandler.Object,
                    dateTimeProvider.Object);
            Stream stream = await messageStreamProviderService.Provide(CancellationToken.None);

            Assert.Null(stream);
            validationErrorHandler.Verify();
            Assert.Equal(expectedRule, foundRule);
        }
    }
}
