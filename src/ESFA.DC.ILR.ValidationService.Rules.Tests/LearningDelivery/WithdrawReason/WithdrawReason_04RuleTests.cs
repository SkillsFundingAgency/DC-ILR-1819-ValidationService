using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_04RuleTests : AbstractRuleTests<WithdrawReason_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WithdrawReason_04");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(6)]
        public void CompStatusConditionMet_True(int compStatus)
        {
            NewRule().CompStatusConditionMet(compStatus).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(10)]
        public void CompStatusConditionMet_False(int compStatus)
        {
            NewRule().CompStatusConditionMet(compStatus).Should().BeFalse();
        }

        [Fact]
        public void WithdrawReasonConditionMet_True()
        {
            var withdrawReason = 1;

            NewRule().WithdrawReasonConditionMet(withdrawReason).Should().BeTrue();
        }

        [Fact]
        public void WithdrawReasonConditionMet_False()
        {
            int? withdrawReason = null;

            NewRule().WithdrawReasonConditionMet(withdrawReason).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(6, 1)]
        public void ConditionMet_True(int compStatus, int? withdrawReason)
        {
            NewRule().ConditionMet(compStatus, withdrawReason).Should().BeTrue();
        }

        [Theory]
        [InlineData(5, null)]
        [InlineData(0, 1)]
        public void ConditionMet_False(int compStatus, int? withdrawReason)
        {
            NewRule().ConditionMet(compStatus, withdrawReason).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(6, 1)]
        public void Validate_Error(int compStatus, int? withdrawReason)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        CompStatus = compStatus,
                        WithdrawReasonNullable = withdrawReason
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(5, null)]
        [InlineData(0, 1)]
        public void Validate_NoError(int compStatus, int? withdrawReason)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        CompStatus = compStatus,
                        WithdrawReasonNullable = withdrawReason
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var compStatus = 1;
            int? withdrawReason = 2;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("CompStatus", compStatus)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("WithdrawReason", withdrawReason)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(compStatus, withdrawReason);

            validationErrorHandlerMock.Verify();
        }

        private WithdrawReason_04Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WithdrawReason_04Rule(validationErrorHandler);
        }
    }
}
