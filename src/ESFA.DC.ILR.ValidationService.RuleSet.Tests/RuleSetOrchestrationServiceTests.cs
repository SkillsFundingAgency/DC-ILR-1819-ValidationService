using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.ExternalData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class RuleSetOrchestrationServiceTests
    {
        public RuleSetOrchestrationService<T, U> NewService<T, U>(
            IRuleSetResolutionService<T> ruleSetResolutionService = null,
            IValidationItemProviderService<T> validationItemProviderService = null,
            IReferenceDataCache referenceDataCache = null,
            IReferenceDataCachePopulationService<T> referenceDataCachePopulationService = null,
            IRuleSetExecutionService<T> ruleSetExecutionService = null,
            IValidationOutputService<U> validationOutputService = null)
            where T : class
        {
            return new RuleSetOrchestrationService<T, U>(ruleSetResolutionService, validationItemProviderService, referenceDataCache, referenceDataCachePopulationService, ruleSetExecutionService, validationOutputService);
        }

        [Fact]
        public void Execute_NoValidationItems()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(), new RuleTwo() });

            var validationItems = new List<string>();

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<string>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide(validationContextMock.Object)).Returns(validationItems);

            var referenceDataCacheMock = new Mock<IReferenceDataCache>();

            var referenceDataCachePopulationServiceMock = new Mock<IReferenceDataCachePopulationService<string>>();
            referenceDataCachePopulationServiceMock.Setup(ps => ps.Populate(referenceDataCacheMock.Object, validationItems));

            var output = new List<int>() { 1, 2, 3 };

            var validationOutputService = new Mock<IValidationOutputService<int>>();
            validationOutputService.Setup(os => os.Process()).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, referenceDataCacheMock.Object, referenceDataCachePopulationServiceMock.Object, validationOutputService: validationOutputService.Object);

            service.Execute(validationContextMock.Object).Should().BeSameAs(output);
        }

        [Fact]
        public void Execute()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSet = new List<IRule<string>>() { new RuleOne(), new RuleTwo() };

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(ruleSet);

            const string one = "one";
            const string two = "two";
            var validationItems = new List<string>() { one, two };

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<string>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide(validationContextMock.Object)).Returns(validationItems);

            var referenceDataCacheMock = new Mock<IReferenceDataCache>();

            var referenceDataCachePopulationServiceMock = new Mock<IReferenceDataCachePopulationService<string>>();
            referenceDataCachePopulationServiceMock.Setup(ps => ps.Populate(referenceDataCacheMock.Object, validationItems));

            var ruleSetExecutionServiceMock = new Mock<IRuleSetExecutionService<string>>();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, one)).Verifiable();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, two)).Verifiable();

            var output = new List<int>() { 1, 2, 3 };

            var validationOutputService = new Mock<IValidationOutputService<int>>();
            validationOutputService.Setup(os => os.Process()).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, referenceDataCacheMock.Object, referenceDataCachePopulationServiceMock.Object, ruleSetExecutionServiceMock.Object, validationOutputService.Object);

            service.Execute(validationContextMock.Object).Should().BeSameAs(output);

            ruleSetExecutionServiceMock.Verify();
        }
    }
}
