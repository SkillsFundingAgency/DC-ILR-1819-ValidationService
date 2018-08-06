using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnActEndDate
{
    public class LearnActEndDate_01RuleTests : AbstractRuleTests<LearnActEndDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnActEndDate_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 2), new DateTime(2017, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Dates()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2)).Should().BeFalse();
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
                        LearnStartDate = new DateTime(2017, 1, 2),
                        LearnActEndDateNullable = new DateTime(2017, 1, 1),
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
                        LearnStartDate = new DateTime(2017, 1, 2),
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "02/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2));

            validationErrorHandlerMock.Verify();
        }

        private LearnActEndDate_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnActEndDate_01Rule(validationErrorHandler);
        }
    }
}
