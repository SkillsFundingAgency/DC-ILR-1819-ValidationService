using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R47RuleTests : AbstractRuleTests<R47Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R47");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testLearner = new TestLearner
            {
                ContactPreferences = new List<IContactPreference>
                {
                    new TestContactPreference
                    {
                        ContPrefType = "12345",
                        ContPrefCode = 1
                    },
                    new TestContactPreference
                    {
                        ContPrefType = "123456",
                        ContPrefCode = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testLearner = new TestLearner
            {
                ContactPreferences = new List<IContactPreference>
                {
                    new TestContactPreference
                    {
                        ContPrefType = "12345",
                        ContPrefCode = 1
                    },
                    new TestContactPreference
                    {
                        ContPrefType = "12345",
                        ContPrefCode = 1
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<int>()));
        }

        private R47Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R47Rule(validationErrorHandler);
        }
    }
}
