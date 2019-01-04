using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class RuleSetOrchestrationServiceTests
    {
        [Fact]
        public async Task Execute_NoValidationItems()
        {
            var output = new List<string> { "1", "2", "3" };

            IValidationErrorCache<string> validationErrorCache = new ValidationErrorCacheGenericTest<string>();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(validationErrorCache), new RuleTwo(validationErrorCache) });

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string> { "NA" });

            var ruleSetExecutionService = new RuleSetExecutionService<string>();

            var service = NewService(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, validationErrorCache: validationErrorCache, ruleSetExecutionService: ruleSetExecutionService);

            (await service.ExecuteAsync(new List<string>(), CancellationToken.None)).Should().BeEquivalentTo(output);
        }

        [Fact]
        public async Task Execute_FilteredValidationItems()
        {
            IValidationErrorCache<string> validationErrorCache = new ValidationErrorCacheGenericTest<string>();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(validationErrorCache), new RuleTwo(validationErrorCache) });

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string> { "NA" });

            var ruleSetExecutionService = new RuleSetExecutionService<string>();

            var service = NewService(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, validationErrorCache: validationErrorCache, ruleSetExecutionService: ruleSetExecutionService);

            var filtered = new List<string> { "RuleOne", "RuleTwo" };

            (await service.ExecuteAsync(filtered, CancellationToken.None)).Should().BeEmpty();
        }

        [Fact]
        public async Task Execute()
        {
            var output = new List<string> { "1", "2", "3", "1", "2", "3" };

            IValidationErrorCache<string> validationErrorCache = new ValidationErrorCacheGenericTest<string>();

            var ruleSet = new List<IRule<string>> { new RuleOne(validationErrorCache), new RuleTwo(validationErrorCache) };

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(ruleSet);

            const string one = "one";
            const string two = "two";
            var validationItems = new List<string> { one, two };

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(validationItems);

            var ruleSetExecutionService = new RuleSetExecutionService<string>();

            var service = NewService(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, ruleSetExecutionService, validationErrorCache);

            (await service.ExecuteAsync(new List<string>(), CancellationToken.None)).Should().BeEquivalentTo(output);
        }

        public RuleSetOrchestrationService<T, U> NewService<T, U>(
            IRuleSetResolutionService<T> ruleSetResolutionService = null,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService = null,
            IRuleSetExecutionService<T> ruleSetExecutionService = null,
            IValidationErrorCache<U> validationErrorCache = null)
            where T : class
        {
            return new RuleSetOrchestrationService<T, U>(
                ruleSetResolutionService,
                validationItemProviderService,
                ruleSetExecutionService,
                validationErrorCache);
        }
    }
}
