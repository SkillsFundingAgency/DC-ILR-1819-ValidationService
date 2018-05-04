using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class PreValidationPopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var messageCachePopulationServiceMock = NewPopulationServiceMock<IMessageCachePopulationService>();
            var fileDataCachePopulationServiceMock = NewPopulationServiceMock<IFileDataCachePopulationService>();
            var internalDataCachePopulationServiceMock = NewPopulationServiceMock<IInternalDataCachePopulationService>();
            var externalDataCachePopulationServiceMock = NewPopulationServiceMock<IExternalDataCachePopulationService>();

            NewService(messageCachePopulationServiceMock.Object, fileDataCachePopulationServiceMock.Object, internalDataCachePopulationServiceMock.Object, externalDataCachePopulationServiceMock.Object).Populate();

            messageCachePopulationServiceMock.Verify();
            fileDataCachePopulationServiceMock.Verify();
            internalDataCachePopulationServiceMock.Verify();
            externalDataCachePopulationServiceMock.Verify();
        }

        private Mock<T> NewPopulationServiceMock<T>()
            where T : class, IPopulationService
        {
            var mock = new Mock<T>();

            mock.Setup(ps => ps.Populate()).Verifiable();

            return mock;
        }

        private PreValidationPopulationService NewService(
            IMessageCachePopulationService messageCachePopulationService = null,
            IFileDataCachePopulationService fileDataCachePopulationService = null,
            IInternalDataCachePopulationService internalDataCachePopulationService = null,
            IExternalDataCachePopulationService externalDataCachePopulationService = null)
        {
            return new PreValidationPopulationService(
                messageCachePopulationService,
                fileDataCachePopulationService,
                internalDataCachePopulationService,
                externalDataCachePopulationService);
        }
    }
}
