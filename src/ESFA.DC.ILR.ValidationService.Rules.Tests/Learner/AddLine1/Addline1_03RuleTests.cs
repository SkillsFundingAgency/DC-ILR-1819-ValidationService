using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.AddLine1;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.AddLine1
{
    public class Addline1_03RuleTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ConditionMet_True(string addLine1)
        {
            var rule = NewRule();

            rule.ConditionMet(addLine1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();

            rule.ConditionMet("test").Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        public void ExcludePlanLearnHoursConditionMet_True(long? planLearnHours)
        {
            var rule = NewRule();

            rule.ExcludeConditionPlannedLearnHours(planLearnHours).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(11)]
        public void ExcludePlanLearnHoursConditionMet_False(long? planLearnHours)
        {
            var rule = NewRule();

            rule.ExcludeConditionPlannedLearnHours(planLearnHours).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(9)]
        public void ExcludeFundMode10Met_False(long? fundModel)
        {
            var rule = NewRule();

            rule.ExcludeConditionFamValueMet(fundModel, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        public void ExcludeFundMode10Met_True(long? fundModel)
        {
            var rule = NewRule();

            rule.ExcludeConditionFamValueMet(fundModel, null).Should().BeTrue();
        }

        [Theory]
        [InlineData("XYZ", "108")]
        [InlineData("SOF", "9999")]
        public void ExcludeFundMode99Met_False(string famType, string famCode)
        {
            var learningDeliveryFams = new[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var rule = NewRule(null, learningDeliveryFAMQueryServiceMock.Object);
            rule.ExcludeConditionFamValueMet(99, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ExcludeFundMode99Met_True()
        {
            var learningDeliveryFams = new[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "108"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "XXXXX",
                    LearnDelFAMCode = "11111"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(null, learningDeliveryFAMQueryServiceMock.Object);

            rule.ExcludeConditionFamValueMet(99, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void Exclude_LearningDeliveriesNull_False()
        {
            var rule = NewRule();
            rule.Exclude(null, null).Should().BeFalse();
        }

        [Fact]
        public void Exclude_True()
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = 99,
                    LearningDeliveryFAMs = new[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = "108"
                        },
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = "XXXXX",
                            LearnDelFAMCode = "11111"
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(null, learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDeliveries, 10).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = 99999,
                    LearningDeliveryFAMs = new[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = "108"
                        },
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = "XXXXX",
                            LearnDelFAMCode = "11111"
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(null, learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDeliveries, 10).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                AddLine1 = null
            };
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("AddLine1_03", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object, null);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                AddLine1 = "test"
            };
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("AddLine1_03", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private AddLine1_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new AddLine1_03Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}