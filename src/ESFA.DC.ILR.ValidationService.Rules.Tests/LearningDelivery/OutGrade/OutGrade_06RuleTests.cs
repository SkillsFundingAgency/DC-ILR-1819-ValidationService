using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OutGrade
{
    public class OutGrade_06RuleTests : AbstractRuleTests<OutGrade_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_06");
        }

        [Fact]
        public void OutcomeConditionMet_True()
        {
            NewRule().OutcomeConditionMet(3).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void OutGradeConditionMet_True()
        {
            NewRule().OutGradeConditionMet("XX").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("FL")]
        [InlineData("U")]
        [InlineData("N")]
        [InlineData("X")]
        [InlineData("Y")]
        public void OutGradeConditionMet_False(string outGrade)
        {
            NewRule().OutGradeConditionMet(outGrade).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outcome = 3;
            var outGrade = "XX";

            NewRule().ConditionMet(outcome, outGrade).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            var outcome = 0;
            var outGrade = "XX";

            NewRule().ConditionMet(outcome, outGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("FL")]
        [InlineData("U")]
        [InlineData("N")]
        [InlineData("X")]
        [InlineData("Y")]
        public void ConditionMet_False_OutGrade(string outGrade)
        {
            var outcome = 3;

            NewRule().ConditionMet(outcome, outGrade).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 3,
                        OutGrade = "XX"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("FL")]
        [InlineData("U")]
        [InlineData("N")]
        [InlineData("X")]
        [InlineData("Y")]
        public void ValidateNoError(string outGrade)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 3,
                        OutGrade = outGrade
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
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutGrade", "XX")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, "XX");

            validationErrorHandlerMock.Verify();
        }

        private OutGrade_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutGrade_06Rule(validationErrorHandler);
        }
    }
}
