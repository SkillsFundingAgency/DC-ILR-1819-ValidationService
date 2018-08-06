using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMDateTo
{
    public class LearnDelFAMDateTo_03RuleTests : AbstractRuleTests<LearnDelFAMDateTo_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMDateTo_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnDelFamDateTo = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var fundModel = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.DatesConditionMet(learnDelFamDateTo, learnActEndDate)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);

            ruleMock.Object.ConditionMet(learnDelFamDateTo, learnActEndDate, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Dates()
        {
            var learnDelFamDateTo = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var fundModel = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.DatesConditionMet(learnDelFamDateTo, learnActEndDate)).Returns(false);

            ruleMock.Object.ConditionMet(learnDelFamDateTo, learnActEndDate, fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var learnDelFamDateTo = new DateTime(2017, 1, 1);
            var learnActEndDate = new DateTime(2017, 1, 1);
            var fundModel = 1;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.DatesConditionMet(learnDelFamDateTo, learnActEndDate)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);

            ruleMock.Object.ConditionMet(learnDelFamDateTo, learnActEndDate, fundModel).Should().BeFalse();
        }

        [Fact]
        public void DatesConditionMet_True()
        {
            NewRule().DatesConditionMet(new DateTime(2017, 1, 1), new DateTime(2016, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void DatesConditionMet_False()
        {
            NewRule().DatesConditionMet(new DateTime(2016, 1, 1), new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void DatesConditionMet_False_LearnDelFamDateToNull()
        {
            NewRule().DatesConditionMet(null, new DateTime(2016, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void DatesConditionMet_False_LearnActEndDateNull()
        {
            NewRule().DatesConditionMet(new DateTime(2016, 1, 1), null).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(99)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
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
                        LearnActEndDateNullable = new DateTime(2016, 1, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMDateToNullable = new DateTime(2017, 1, 1),
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
                        FundModel = 1,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMDateToNullable = null
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/01/2016")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2016, 1, 1), new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMDateTo_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMDateTo_03Rule(validationErrorHandler);
        }
    }
}
