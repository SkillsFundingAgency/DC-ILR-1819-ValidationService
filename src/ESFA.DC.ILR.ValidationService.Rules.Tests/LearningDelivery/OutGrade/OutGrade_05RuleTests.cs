using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OutGrade
{
    public class OutGrade_05RuleTests : AbstractRuleTests<OutGrade_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_05");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(8)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        public void OutcomeConditionMet_True(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        public void OutGradeConditionMet_False(string outGrade)
        {
            NewRule().OutGradeConditionMet(outGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("FL")]
        [InlineData("U")]
        [InlineData("N")]
        [InlineData("X")]
        [InlineData("Y")]
        public void OutGradeConditionMet_True(string outGrade)
        {
            NewRule().OutGradeConditionMet(outGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData("FL", 2)]
        [InlineData("FL", 3)]
        [InlineData("FL", 8)]
        [InlineData("U", 2)]
        [InlineData("U", 3)]
        [InlineData("U", 8)]
        [InlineData("N", 2)]
        [InlineData("N", 3)]
        [InlineData("N", 8)]
        [InlineData("X", 2)]
        [InlineData("X", 3)]
        [InlineData("X", 8)]
        [InlineData("Y", 2)]
        [InlineData("Y", 3)]
        [InlineData("Y", 8)]
        public void ConditionMet_False(string outgrade, int outcome)
        {
            NewRule().ConditionMet(outcome, outgrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("FL", 1)]
        [InlineData("U", 1)]
        [InlineData("N", 1)]
        [InlineData("X", 1)]
        [InlineData("Y", 1)]
        public void ConditionMet_True(string outgrade, int outcome)
        {
            NewRule().ConditionMet(outcome, outgrade).Should().BeTrue();
        }

        [Theory]
        [InlineData("FL", 2)]
        [InlineData("FL", 3)]
        [InlineData("FL", 8)]
        [InlineData("U", 2)]
        [InlineData("U", 3)]
        [InlineData("U", 8)]
        [InlineData("N", 2)]
        [InlineData("N", 3)]
        [InlineData("N", 8)]
        [InlineData("X", 2)]
        [InlineData("X", 3)]
        [InlineData("X", 8)]
        [InlineData("Y", 2)]
        [InlineData("Y", 3)]
        [InlineData("Y", 8)]
        public void Validate_NoError(string outGrade, int outcome)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = outcome,
                        OutGrade = outGrade
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("FL", 1)]
        [InlineData("U", 1)]
        [InlineData("N", 1)]
        [InlineData("X", 1)]
        [InlineData("Y", 1)]
        public void Validate_Error(string outGrade, int outcome)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = outcome,
                        OutGrade = outGrade
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.Outcome, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.OutGrade, "XX")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, "XX");

            validationErrorHandlerMock.Verify();
        }

        private OutGrade_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutGrade_05Rule(validationErrorHandler);
        }
    }
}
