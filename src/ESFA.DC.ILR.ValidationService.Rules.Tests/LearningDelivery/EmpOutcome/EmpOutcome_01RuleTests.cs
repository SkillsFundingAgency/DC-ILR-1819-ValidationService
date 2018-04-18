using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_01RuleTests : AbstractRuleTests<EmpOutcome_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpOutcome_01");
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        [InlineData(99)]
        public void ConditionMet_True(int fundModel)
        {
            NewRule().ConditionMet(1, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null, 25).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(1, 1).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EmpOutcomeNullable = 1,
                        FundModel = 10,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EmpOutcomeNullable = null
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpOutcome", 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1);

            validationErrorHandlerMock.Verify();
        }

        private EmpOutcome_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpOutcome_01Rule(validationErrorHandler);
        }
    }
}
