using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.FamilyName
{
    public class FamilyName_04RuleTests
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

            NewRule(learningDeliveryFAMQueryServiceMock.Object).CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
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
        public void ConditionMet_True(long planLearnHours, string familyName)
        {
            NewRule().ConditionMet(planLearnHours, 1, familyName).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_PlanLearnHours()
        {
            NewRule().ConditionMet(11, 1, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Uln()
        {
            NewRule().ConditionMet(3, 9999999999, null);
        }

        [Fact]
        public void ConditionMet_False_FamilyName()
        {
            NewRule().ConditionMet(3, 1, "Geoff").Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 3,
                FamilyName = null,
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

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("FamilyName_04", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);

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

        private FamilyName_04Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FamilyName_04Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
