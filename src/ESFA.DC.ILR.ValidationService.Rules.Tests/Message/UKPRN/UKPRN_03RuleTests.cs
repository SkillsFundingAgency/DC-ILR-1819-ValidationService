using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Message.UKPRN
{
    public class UKPRN_03RuleTests : AbstractRuleTests<UKPRN_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_03");
        }

        [Theory]
        [InlineData(11111111, 99999999)]
        [InlineData(99999999, 11111111)]
        public void ConditionMet_True(int sourceUKPRN, int learningProviderUKPRN)
        {
            NewRule().ConditionMet(sourceUKPRN, learningProviderUKPRN).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var sourceUKPRN = 99999999;
            var learningProviderUKPN = 99999999;

            NewRule().ConditionMet(sourceUKPRN, learningProviderUKPN).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var message = new TestMessage
            {
                HeaderEntity = new TestHeader
                {
                    SourceEntity = new TestSource
                    {
                        UKPRN = 11111111
                    }
                },
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 99999999
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var message = new TestMessage
            {
                HeaderEntity = new TestHeader
                {
                    SourceEntity = new TestSource
                    {
                        UKPRN = 99999999
                    }
                },
                LearningProviderEntity = new TestLearningProvider
                {
                    UKPRN = 99999999
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("UKPRN", 99999998)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(99999998);

            validationErrorHandlerMock.Verify();
        }

        public UKPRN_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_03Rule(validationErrorHandler);
        }
    }
}
