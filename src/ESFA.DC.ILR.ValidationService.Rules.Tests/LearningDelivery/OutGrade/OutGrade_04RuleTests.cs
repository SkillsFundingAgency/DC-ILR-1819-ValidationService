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
    public class OutGrade_04RuleTests : AbstractRuleTests<OutGrade_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutGrade_04");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(8)]
        public void ConditionMet_True(int? outcome)
        {
            NewRule().ConditionMet("X", outcome).Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ConditionMet_False_OutGrade(string outGrade)
        {
            NewRule().ConditionMet(outGrade, 1).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ConditionMet_False_Outcome(int? outcome)
        {
            NewRule().ConditionMet("X", outcome).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutGrade = "X",
                        OutcomeNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutGrade = "X",
                        OutcomeNullable = 1
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", 8)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutGrade", "XX")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(8, "XX");

            validationErrorHandlerMock.Verify();
        }

        private OutGrade_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutGrade_04Rule(validationErrorHandler);
        }
    }
}
