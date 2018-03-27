using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.GivenNames
{
    public class GivenNames_02RuleTests
    {
        [Fact]
        public void CrossLearningDeliveryConditionMet_True_FundModel10()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = 10
                },
                new TestLearningDelivery()
                {
                    FundModelNullable = 10
                }
            };

            NewRule().CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void CrossLearningDeliveryConditionMet_True_FundModel99()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = 99,
                    LearningDeliveryFAMs = new ILR.Tests.Model.TestLearningDeliveryFAM[] { }
                },
                new TestLearningDelivery()
                {
                    FundModelNullable = 99,
                    LearningDeliveryFAMs = new ILR.Tests.Model.TestLearningDeliveryFAM[] { }
                },
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object, null);

            rule.CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void CrossLearningDeliveryConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = 10
                },
                new TestLearningDelivery()
                {
                    FundModelNullable = 99,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                }
            };

            NewRule().CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void CrossLearningDeliveryConditionMet_False_Null()
        {
            NewRule().CrossLearningDeliveryConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(11, null)]
        [InlineData(1000, null)]
        [InlineData(11, "   ")]
        public void ConditionMet_True(long planLearnHours, string givenNames)
        {
            NewRule().ConditionMet(planLearnHours, givenNames).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_PlanLearnHours()
        {
            NewRule().ConditionMet(3, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_GivenNames()
        {
            NewRule().ConditionMet(11, "Geoff").Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 11,
                GivenNames = null,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 10
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("GivenNames_02", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(validationErrorHandler: validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 8
            };

            NewRule().Validate(learner);
        }

        private GivenNames_02Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new GivenNames_02Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
