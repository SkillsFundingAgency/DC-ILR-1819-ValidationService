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
    public class LearnDelFAMDateFrom_01RuleTests : AbstractRuleTests<LearnDelFAMDateFrom_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMDateFrom_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnDelFamType = "type";
            var learnDelFamTo = new DateTime(2017, 1, 1);
            var learnDelFamFrom = new DateTime(2017, 1, 1);

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.LearnDelFAMTypeConditionMet(learnDelFamType)).Returns(true);
            ruleMock.Setup(r => r.LearnDelFAMDatesConditionMet(learnDelFamFrom, learnDelFamTo)).Returns(true);

            ruleMock.Object.ConditionMet(learnDelFamType, learnDelFamFrom, learnDelFamTo).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Type()
        {
            var learnDelFamType = "type";
            var learnDelFamTo = new DateTime(2017, 1, 1);
            var learnDelFamFrom = new DateTime(2017, 1, 1);

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.LearnDelFAMTypeConditionMet(learnDelFamType)).Returns(false);

            ruleMock.Object.ConditionMet(learnDelFamType, learnDelFamFrom, learnDelFamTo).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Dates()
        {
            var learnDelFamType = "type";
            var learnDelFamTo = new DateTime(2017, 1, 1);
            var learnDelFamFrom = new DateTime(2017, 1, 1);

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.LearnDelFAMTypeConditionMet(learnDelFamType)).Returns(true);
            ruleMock.Setup(r => r.LearnDelFAMDatesConditionMet(learnDelFamFrom, learnDelFamTo)).Returns(false);

            ruleMock.Object.ConditionMet(learnDelFamType, learnDelFamFrom, learnDelFamTo).Should().BeFalse();
        }

        [Theory]
        [InlineData("ALB")]
        [InlineData("LSF")]
        public void LearnDelFamTypeConditionMet_True(string learnDelFamType)
        {
            NewRule().LearnDelFAMTypeConditionMet(learnDelFamType).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamTypeConditionMet_False()
        {
            NewRule().LearnDelFAMTypeConditionMet("NOT").Should().BeFalse();
        }

        [Fact]
        public void LearnDelFamDatesConditionMet_True_From()
        {
            NewRule().LearnDelFAMDatesConditionMet(null, new DateTime(2017, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamDatesConditionMet_True_To()
        {
            NewRule().LearnDelFAMDatesConditionMet(new DateTime(2017, 1, 1), null).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamDatesConditionMet_True_Both()
        {
            NewRule().LearnDelFAMDatesConditionMet(null, null).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamDatesConditionMet_False()
        {
            NewRule().LearnDelFAMDatesConditionMet(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1)).Should().BeFalse();
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
                                LearnDelFAMType = "ALB",
                                LearnDelFAMDateFromNullable = null
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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("Type", new DateTime(2016, 1, 1),  new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMDateFrom_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMDateFrom_01Rule(validationErrorHandler);
        }
    }
}
