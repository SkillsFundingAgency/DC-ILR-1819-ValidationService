using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class RuleSetOrchestrationServiceTests
    {
        [Fact]
        public void Execute_NoValidationItems()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(), new RuleTwo() });

            var messageCachePopulationServiceMock = new Mock<IMessageCachePopulationService>();

            var validationItems = new List<string>();

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide()).Returns(validationItems);

            var referenceDataCachePopulationServiceMock = new Mock<IExternalDataCachePopulationService<string>>();
            referenceDataCachePopulationServiceMock.Setup(ps => ps.Populate(validationItems));

            var internalDataCachePopulationServiceMock = new Mock<IInternalDataCachePopulationService>();
            internalDataCachePopulationServiceMock.Setup(ps => ps.Populate());

            var fileDataCachePopulationServiceMock = new Mock<IFileDataCachePopulationService>();
            fileDataCachePopulationServiceMock.Setup(ps => ps.Populate());

            var output = new List<int>() { 1, 2, 3 };

            var validationOutputService = new Mock<IValidationOutputService<int>>();
            validationOutputService.Setup(os => os.Process()).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, messageCachePopulationServiceMock.Object, validationItemProviderServiceMock.Object, referenceDataCachePopulationServiceMock.Object, internalDataCachePopulationServiceMock.Object, fileDataCachePopulationServiceMock.Object, validationOutputService: validationOutputService.Object);

            service.Execute(validationContextMock.Object).Should().BeSameAs(output);
        }

        [Fact]
        public void Execute()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSet = new List<IRule<string>>() { new RuleOne(), new RuleTwo() };

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(ruleSet);

            var messageCachePopulationServiceMock = new Mock<IMessageCachePopulationService>();
            messageCachePopulationServiceMock.Setup(ps => ps.Populate());

            const string one = "one";
            const string two = "two";
            var validationItems = new List<string>() { one, two };

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide()).Returns(validationItems);

            var referenceDataCachePopulationServiceMock = new Mock<IExternalDataCachePopulationService<string>>();
            referenceDataCachePopulationServiceMock.Setup(ps => ps.Populate(validationItems));

            var internalDataCachePopulationServiceMock = new Mock<IInternalDataCachePopulationService>();
            internalDataCachePopulationServiceMock.Setup(ps => ps.Populate());

            var fileDataCachePopulationServiceMock = new Mock<IFileDataCachePopulationService>();
            fileDataCachePopulationServiceMock.Setup(ps => ps.Populate());

            var ruleSetExecutionServiceMock = new Mock<IRuleSetExecutionService<string>>();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, one)).Verifiable();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, two)).Verifiable();

            var output = new List<int>() { 1, 2, 3 };

            var validationOutputService = new Mock<IValidationOutputService<int>>();
            validationOutputService.Setup(os => os.Process()).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, messageCachePopulationServiceMock.Object, validationItemProviderServiceMock.Object, referenceDataCachePopulationServiceMock.Object, internalDataCachePopulationServiceMock.Object, fileDataCachePopulationServiceMock.Object, ruleSetExecutionServiceMock.Object, validationOutputService.Object);

            service.Execute(validationContextMock.Object).Should().BeSameAs(output);

            ruleSetExecutionServiceMock.Verify();
        }

        public RuleSetOrchestrationService<T, U> NewService<T, U>(
            IRuleSetResolutionService<T> ruleSetResolutionService = null,
            IMessageCachePopulationService messageCachePopulationService = null,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService = null,
            IExternalDataCachePopulationService<T> referenceDataCachePopulationService = null,
            IInternalDataCachePopulationService internalDataCachePopulationService = null,
            IFileDataCachePopulationService fileDataCachePopulationService = null,
            IRuleSetExecutionService<T> ruleSetExecutionService = null,
            IValidationOutputService<U> validationOutputService = null)
            where T : class
        {
            return new RuleSetOrchestrationService<T, U>(
                ruleSetResolutionService,
                messageCachePopulationService,
                validationItemProviderService,
                referenceDataCachePopulationService,
                internalDataCachePopulationService,
                fileDataCachePopulationService,
                ruleSetExecutionService,
                validationOutputService);
        }
    }
}
