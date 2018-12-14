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
    public class R71RuleTests : AbstractRuleTests<R71Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R71");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345"
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "123456"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345"
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345"
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
            validationErrorHandlerMock.Verify(h => h.Handle("R71", "12345", null, null));
        }

        private R71Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R71Rule(validationErrorHandler);
        }
    }
}
