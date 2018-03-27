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
    public class GivenNames_04RuleTests
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
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                },
                new TestLearningDelivery()
                {
                    FundModelNullable = 99,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

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
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMCode = "SOF",
                            LearnDelFAMType = "108"
                        }
                    }
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
        [InlineData(3, null)]
        [InlineData(0, null)]
        [InlineData(4, "   ")]
        public void ConditionMet_True(long planLearnHours, string givenNames)
        {
            NewRule().ConditionMet(planLearnHours, 1, givenNames).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(11)]
        public void ConditionMet_False_PlanLearnHours(long? planLearnHours)
        {
            NewRule().ConditionMet(planLearnHours, 1, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(9999999999)]
        public void ConditionMet_False_Uln(long? uln)
        {
            NewRule().ConditionMet(3, uln, null);
        }

        [Fact]
        public void ConditionMet_False_GivenNames()
        {
            NewRule().ConditionMet(3, 1, "Geoff").Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 3,
                GivenNames = null,
                ULNNullable = 1,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 10
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("GivenNames_04", null, null, null);

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
                PlanLearnHoursNullable = 12
            };

            NewRule().Validate(learner);
        }

        private GivenNames_04Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new GivenNames_04Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
