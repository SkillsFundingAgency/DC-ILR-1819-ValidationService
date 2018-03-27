using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_01RuleTests : PlanLearnHoursTestsBase
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(10).Should().BeFalse();
        }

        [Fact]
        public void ExcludeConditionMet_AllAimsClosed_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    LearnActEndDateNullable = new DateTime(2018, 1, 1)
                },
                new TestLearningDelivery()
                {
                    LearnActEndDateNullable = new DateTime(2018, 1, 1)
                }
            };

            NewRule().HasAllLearningAimsClosedExcludeConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ExcludeConditionMet_All_AimsClosed_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery(),
                new TestLearningDelivery()
                {
                    LearnActEndDateNullable = new DateTime(2017, 1, 1),
                }
            };

            NewRule().HasAllLearningAimsClosedExcludeConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(70, null)]
        [InlineData(null, 2)]
        public void ExcludeConditionMet_True(long? fundModel, long? progType)
        {
            var learningDelivery = SetupLearningDelivery(fundModel, progType);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("Y");

            var rule = NewRule(dd07Mock.Object);

            rule.Exclude(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(10, null)]
        [InlineData(null, 12)]
        public void ExcludeConditionMet_False(long? fundModel, long? progType)
        {
            var learningDelivery = SetupLearningDelivery(fundModel, progType);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dd => dd.Derive(progType)).Returns("N");

            var rule = NewRule(dd07Mock.Object);

            rule.Exclude(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ExcludeConditionDD07_False()
        {
            NewRule().HasLearningDeliveryDd07ExcludeConditionMet(string.Empty).Should().BeFalse();
        }

        [Fact]
        public void ExcludeConditionDD07_True()
        {
            NewRule().HasLearningDeliveryDd07ExcludeConditionMet("Y").Should().BeTrue();
        }

        [Fact]
        public void ExcludeConditionFundModel_True()
        {
            NewRule().HasLearningDeliveryFundModelExcludeConditionMet(70).Should().BeTrue();
        }

        [Fact]
        public void ExcludeConditionFundModel_False()
        {
            NewRule().HasLearningDeliveryFundModelExcludeConditionMet(10).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(null, null, 35);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_01", null, null, null);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dd => dd.Derive(It.IsAny<long>())).Returns("N");

            var rule = NewRule(dd07Mock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner(1, null, 35);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PlanLearnHours_01", null, null, null);

            var rule = NewRule(validationErrorHandler: validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private PlanLearnHours_01Rule NewRule(IDD07 dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_01Rule(dd07, validationErrorHandler);
        }

        private ILearningDelivery SetupLearningDelivery(long? fundModel, long? progType)
        {
            return new TestLearningDelivery()
            {
                FundModelNullable = fundModel,
                ProgTypeNullable = progType,
            };
        }
    }
}
