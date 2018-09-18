using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task Execute_NoValidationItems()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(), new RuleTwo() });

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string>());

            var output = new List<int>() { 1, 2, 3 };

            var validationErrorCacheMock = new Mock<IValidationErrorCache<int>>();
            validationErrorCacheMock.SetupGet(c => c.ValidationErrors).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, validationErrorCache: validationErrorCacheMock.Object);

            (await service.Execute(CancellationToken.None)).Should().BeSameAs(output);
        }

        [Fact]
        public async Task Execute()
        {
            var validationContextMock = new Mock<IValidationContext>();

            var ruleSet = new List<IRule<string>>() { new RuleOne(), new RuleTwo() };

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(ruleSet);

            const string one = "one";
            const string two = "two";
            var validationItems = new List<string>() { one, two };

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.ProvideAsync(It.IsAny<CancellationToken>())).ReturnsAsync(validationItems);

            var ruleSetExecutionServiceMock = new Mock<IRuleSetExecutionService<string>>();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, one)).Verifiable();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, two)).Verifiable();

            var output = new List<int>() { 1, 2, 3 };

            var validationErrorCacheMock = new Mock<IValidationErrorCache<int>>();
            validationErrorCacheMock.SetupGet(c => c.ValidationErrors).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, ruleSetExecutionServiceMock.Object, validationErrorCacheMock.Object);

            (await service.Execute(CancellationToken.None)).Should().BeSameAs(output);

            ruleSetExecutionServiceMock.Verify();
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
