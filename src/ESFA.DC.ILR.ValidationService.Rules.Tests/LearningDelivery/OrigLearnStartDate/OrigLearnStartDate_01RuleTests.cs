using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_01RuleTests : AbstractRuleTests<OrigLearnStartDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_01");
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

        [Theory]
        [InlineData(25)]
        [InlineData(88)]
        [InlineData(10)]
        [InlineData(100)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_False_Null()
        {
            NewRule().OriginalLearnStartDateConditionMet(DateTime.MaxValue, null).Should().BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_False()
        {
            var startDate = new DateTime(2018, 10, 10);
            var originalStartDate = new DateTime(2008, 10, 10);
            NewRule().OriginalLearnStartDateConditionMet(startDate, originalStartDate).Should().BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_True()
        {
            var startDate = new DateTime(2018, 10, 10);
            var originalStartDate = new DateTime(2008, 10, 09);
            NewRule().OriginalLearnStartDateConditionMet(startDate, originalStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, "2008-10-09")]
        [InlineData(36, "2008-09-10")]
        [InlineData(81, "2008-10-09")]
        [InlineData(99, "2000-10-10")]
        public void ConditionMet_True(int fundModel, string originalStartDateString)
        {
            var learnStartDate = new DateTime(2018, 10, 10);
            var origLearnStartDate = DateTime.Parse(originalStartDateString);

            NewRule().ConditionMet(learnStartDate, origLearnStartDate, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(88)]
        public void ConditionMet_FundModel_False(int fundModel)
        {
            var learnStartDate = new DateTime(2018, 10, 10);
            var origLearnStartDate = new DateTime(2018, 10, 09);

            NewRule().ConditionMet(learnStartDate, origLearnStartDate, fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData("2008-10-10")]
        [InlineData("2008-11-10")]
        [InlineData("2008-10-11")]
        public void ConditionMet_Startdate_False(string originalStartDateString)
        {
            var learnStartDate = new DateTime(2018, 10, 10);
            var origLearnStartDate = DateTime.Parse(originalStartDateString);

            NewRule().ConditionMet(learnStartDate, origLearnStartDate, 35).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnStartDate = new DateTime(2018, 10, 10);
            var origLearnStartDate = new DateTime(2008, 10, 01);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = 35
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
            var learnStartDate = new DateTime(2018, 10, 10);
            var origLearnStartDate = new DateTime(2008, 10, 10);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = 35
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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OrigLearnStartDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private OrigLearnStartDate_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_01Rule(validationErrorHandler);
        }
    }
}
