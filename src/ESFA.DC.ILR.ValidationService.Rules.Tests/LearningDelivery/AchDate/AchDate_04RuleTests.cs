using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_04RuleTests : AbstractRuleTests<AchDate_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2019, 1, 1), null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2016, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2018, 1, 1)).Should().BeFalse();
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
                        AchDateNullable = new DateTime(2019, 1, 1),
                        LearnActEndDateNullable = null,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private AchDate_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_04Rule(validationErrorHandler);
        }
    }
}
