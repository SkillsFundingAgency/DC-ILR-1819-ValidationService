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
    public class LearnDelFAMDateFrom_03RuleTests : AbstractRuleTests<LearnDelFAMDateFrom_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMDateFrom_03");
        }

        [Theory]
        [InlineData("LSF")]
        [InlineData("ACT")]
        [InlineData("ALB")]
        public void ConditionMet_False(string learnDelFamType)
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null, "Doesn't Matter").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), "NOT").Should().BeTrue();
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
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "NOT",
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "Type")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("Type", new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMDateFrom_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMDateFrom_03Rule(validationErrorHandler);
        }
    }
}
