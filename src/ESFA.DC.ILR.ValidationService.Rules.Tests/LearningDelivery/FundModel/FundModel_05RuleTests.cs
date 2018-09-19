using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_05RuleTests : AbstractRuleTests<FundModel_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_05");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(36, new DateTime(2016, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(10, new DateTime(2016, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            NewRule().ConditionMet(36, new DateTime(2018, 1, 1)).Should().BeFalse();
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
                        FundModel = 36,
                        LearnStartDate = new DateTime(2016, 1, 1),
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
                        FundModel = 10,
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
            var fundModel = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private FundModel_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_05Rule(validationErrorHandler);
        }
    }
}
