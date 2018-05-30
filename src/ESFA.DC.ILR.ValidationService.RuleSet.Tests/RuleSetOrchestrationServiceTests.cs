using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide()).Returns(new List<string>());

            var preValidationPopulationServiceMock = new Mock<IPreValidationPopulationService<IValidationContext>>();
            preValidationPopulationServiceMock.Setup(ps => ps.Populate(It.IsAny<IValidationContext>()));

            var output = new List<int>() { 1, 2, 3 };

            var validationErrorCacheMock = new Mock<IValidationErrorCache<int>>();
            validationErrorCacheMock.SetupGet(c => c.ValidationErrors).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, preValidationPopulationServiceMock.Object, validationErrorCache: validationErrorCacheMock.Object);

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

            var validationItemProviderServiceMock = new Mock<IValidationItemProviderService<IEnumerable<string>>>();
            validationItemProviderServiceMock.Setup(ps => ps.Provide()).Returns(validationItems);

            var preValidationPopulationServiceMock = new Mock<IPreValidationPopulationService<IValidationContext>>();
            preValidationPopulationServiceMock.Setup(ps => ps.Populate(It.IsAny<IValidationContext>()));

            var ruleSetExecutionServiceMock = new Mock<IRuleSetExecutionService<string>>();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, one)).Verifiable();
            ruleSetExecutionServiceMock.Setup(es => es.Execute(ruleSet, two)).Verifiable();

            var output = new List<int>() { 1, 2, 3 };

            var validationErrorCacheMock = new Mock<IValidationErrorCache<int>>();
            validationErrorCacheMock.SetupGet(c => c.ValidationErrors).Returns(output);

            var service = NewService<string, int>(ruleSetResolutionServiceMock.Object, validationItemProviderServiceMock.Object, preValidationPopulationServiceMock.Object, ruleSetExecutionServiceMock.Object, validationErrorCacheMock.Object);

            service.Execute(validationContextMock.Object).Should().BeSameAs(output);

            ruleSetExecutionServiceMock.Verify();
        }

        public RuleSetOrchestrationService<T, U> NewService<T, U>(
            IRuleSetResolutionService<T> ruleSetResolutionService = null,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService = null,
            IPreValidationPopulationService<IValidationContext> preValidationPopulationService = null,
            IRuleSetExecutionService<T> ruleSetExecutionService = null,
            IValidationErrorCache<U> validationErrorCache = null)
            where T : class
        {
            return new RuleSetOrchestrationService<T, U>(
                ruleSetResolutionService,
                validationItemProviderService,
                preValidationPopulationService,
                ruleSetExecutionService,
                validationErrorCache);
        }
    }
}
