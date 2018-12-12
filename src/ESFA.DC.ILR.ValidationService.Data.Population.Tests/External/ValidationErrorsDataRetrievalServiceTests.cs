using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.External
{
    public class ValidationErrorsDataRetrievalServiceTests
    {
        [Fact]
        public async Task RetrieveAsync()
        {
            var rules = new List<Rule>()
            {
                new Rule()
                {
                    Rulename = "RuleName1",
                    Message = "Rule Name 1 Message",
                    Severity = "E"
                },
                new Rule()
                {
                    Rulename = "RuleName2",
                    Message = "Rule Name 2 Message",
                    Severity = "W"
                },
                new Rule()
                {
                    Rulename = "RuleName3",
                    Message = "Rule Name 3 Message",
                    Severity = "F"
                }
            }.AsMockDbSet();

            var validationErrorsMock = new Mock<IValidationErrors>();

            validationErrorsMock.Setup(v => v.Rules).Returns(rules);

            var validations = await this.NewService(validationErrors: validationErrorsMock.Object).RetrieveAsync(CancellationToken.None);
            validations.Should().HaveCount(3);
            validations.Keys.Should().Contain("rulename1"); // case insensitivity checks
            validations.Keys.Should().Contain("RULENAME2"); // case insensitivity checks
            validations.Keys.Should().Contain("ruleName3"); // case insensitivity checks
        }

        [Fact]
        public async Task RetrieveAsync_EmptyCheck()
        {
            var rules = new List<Rule>();
            var rulesDBContextMock = rules.AsMockDbSet();

            var validationErrorsMock = new Mock<IValidationErrors>();

            validationErrorsMock.Setup(v => v.Rules).Returns(rulesDBContextMock);

            var validations = await this.NewService(validationErrors: validationErrorsMock.Object).RetrieveAsync(CancellationToken.None);

            validations.Should().BeNullOrEmpty();
        }

            private ValidationErrorsDataRetrievalService NewService(IValidationErrors validationErrors = null)
        {
            return new ValidationErrorsDataRetrievalService(validationErrors: validationErrors);
        }
    }
}
