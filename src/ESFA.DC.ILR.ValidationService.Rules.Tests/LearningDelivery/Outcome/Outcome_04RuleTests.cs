using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_04RuleTests : AbstractRuleTests<Outcome_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_04");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(2)]
        public void OutcomeConditionMet_True(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeTrue();
        }

        [Fact]
        public void OutcomeConditionMet_False()
        {
            var outcome = 1;

            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_True()
        {
            var achDate = new DateTime(2018, 01, 01);

            NewRule().AchDateConditionMet(achDate).Should().BeTrue();
        }

        [Fact]
        public void AchDateConditionMet_False()
        {
            DateTime? achDate = null;

            NewRule().AchDateConditionMet(achDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outcome = 5;
            var achDate = new DateTime(2018, 01, 01);

            NewRule().ConditionMet(achDate, outcome).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseAchDate()
        {
            var outcome = 5;

            NewRule().ConditionMet(null, outcome).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseOutcome()
        {
            var outcome = 1;
            var achDate = new DateTime(2018, 01, 01);

            NewRule().ConditionMet(achDate, outcome).Should().BeFalse();
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
                        OutcomeNullable = 5,
                        AchDateNullable = new DateTime(2018, 01, 01)
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
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 8,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/10/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01), 1);

            validationErrorHandlerMock.Verify();
        }

        private Outcome_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Outcome_04Rule(validationErrorHandler);
        }
    }
}
