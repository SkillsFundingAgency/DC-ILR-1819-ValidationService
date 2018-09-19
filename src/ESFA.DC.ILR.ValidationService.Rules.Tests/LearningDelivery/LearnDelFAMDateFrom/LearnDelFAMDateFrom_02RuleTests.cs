using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateFrom;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMDateFrom
{
    public class LearnDelFAMDateFrom_02RuleTests : AbstractRuleTests<LearnDelFAMDateFrom_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMDateFrom_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2016, 1, 1), new DateTime(2017, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2016, 1, 1)).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(99)]
        public void LearnDelFamTypeConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamTypeConditionMet_False()
        {
            NewRule().FundModelConditionMet(1).Should().BeFalse();
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
                        FundModel = 35,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMDateFromNullable = new DateTime(2016, 1, 1)
                            }
                        }
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
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "NOT",
                                LearnDelFAMDateFromNullable = null
                            }
                        }
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 1, new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMDateFrom_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMDateFrom_02Rule(validationErrorHandler);
        }
    }
}
