using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_07RuleTests
    {
        [Fact]
        public void ConditionMet_True_NullLLDHealthAndProblems()
        {
            var rule = NewRule();
            rule.ConditionLLDDHealthAndProblemsMet(null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Empty()
        {
            var rule = NewRule();
            rule.ConditionLLDDHealthAndProblemsMet(new List<ILLDDAndHealthProblem>()).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2)]
        public void ConditionMetHealthProblem_False(long? lldHealthProblem)
        {
            var rule = NewRule();
            rule.ConditionLLDHealthConditionMet(lldHealthProblem).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetHealthProblem_True()
        {
            var rule = NewRule();
            rule.ConditionLLDHealthConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void ConditionPlanLearnHoursMet_True()
        {
            var rule = NewRule();
            rule.ConditionPlannedLearnHoursMet(11).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(10)]
        public void ConditionPlanLearnHoursMet_False(long? planLearnHours)
        {
            var rule = NewRule();
            rule.ConditionPlannedLearnHoursMet(planLearnHours).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2)]
        public void ConditionMet_False(long? llddHealthProblem)
        {
            var rule = NewRule();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            rule.ConditionMet(llddHealthProblem, It.IsAny<long>(), It.IsAny<IReadOnlyCollection<ILLDDAndHealthProblem>>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            rule.ConditionMet(1, It.IsAny<long>(), It.IsAny<IReadOnlyCollection<ILLDDAndHealthProblem>>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(9)]
        public void ConditionFundMode10Met_False(long? fundModel)
        {
            var rule = NewRule();

            rule.ConditionFamValueMet(fundModel, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        public void ConditionFundMode10Met_True(long? fundModel)
        {
            var rule = NewRule();

            rule.ConditionFamValueMet(fundModel, null).Should().BeTrue();
        }

        [Theory]
        [InlineData("XYZ", "108")]
        [InlineData("SOF", "9999")]
        public void ConditionFundModel99Met_False(string famType, string famCode)
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

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);
            rule.ConditionFamValueMet(99, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionFundMode99Met_True()
        {
            var learningDeliveryFams = SetupLearningDeliveries(It.IsAny<long>()).First().LearningDeliveryFAMs;

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);

            rule.ConditionFamValueMet(99, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionLearningDeliveriesNull_False()
        {
            var rule = NewRule(null);
            rule.LearningDeliveriesConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionLearningDeliveries_False()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);
            var learningDeliveries = SetupLearningDeliveries(108);
            rule.LearningDeliveriesConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionLearningDeliveries_True()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);

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
                        }
                    }
                },
                new TestLearningDelivery()
                {
                    FundModelNullable = 10
                }
            };

            rule.LearningDeliveriesConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("1996-01-01", null)]
        [InlineData(null, "2017-10-10")]
        [InlineData("1992-10-11", "2017-10-10")]
        public void Exclude_False(string dateOfBirth, string minimumLearnStart)
        {
            var dateofBirthDate = string.IsNullOrEmpty(dateOfBirth) ? (DateTime?)null : DateTime.Parse(dateOfBirth);
            var minimumLearnStartDate = string.IsNullOrEmpty(minimumLearnStart) ? (DateTime?)null : DateTime.Parse(minimumLearnStart);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(24);

            var rule = NewRule(null, null, null, dateTimeQueryServiceMock.Object);

            rule.Exclude(dateofBirthDate, minimumLearnStartDate).Should().BeFalse();
        }

        [Fact]
        public void Exclude_True()
        {
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(new DateTime(1992, 10, 10), new DateTime(2017, 10, 10))).Returns(25);
            var rule = NewRule(null, null, null, dateTimeQueryServiceMock.Object);

            rule.Exclude(new DateTime(1992, 10, 10), new DateTime(2017, 10, 10)).Should().BeTrue();
        }

        [Fact]
        public void Validate_False()
        {
            var validationErrorHandlerMock = SetupLearnerForValidate(null);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_07", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupLearnerForValidate(new List<ILLDDAndHealthProblem>() { new TestLLDDAndHealthProblem() });
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_07", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Mock<IValidationErrorHandler> SetupLearnerForValidate(IReadOnlyCollection<ILLDDAndHealthProblem> lldHealthProblems)
        {
            var learner = new TestLearner()
            {
                LLDDHealthProbNullable = 1,
                PlanLearnHoursNullable = 11,
                LearningDeliveries = SetupLearningDeliveries(10),
                LLDDAndHealthProblems = lldHealthProblems
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "XYZ", "108")).Returns(false);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2018, 10, 10));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(24);

            var rule = NewRule(validationErrorHandlerMock.Object, dd06Mock.Object, learningDeliveryFAMQueryServiceMock.Object, dateTimeQueryServiceMock.Object);
            rule.Validate(learner);

            return validationErrorHandlerMock;
        }

        private LLDDHealthProb_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IDD06 dd06 = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LLDDHealthProb_07Rule(validationErrorHandler, dd06, learningDeliveryFAMQueryService, dateTimeQueryService);
        }

        private TestLearningDelivery[] SetupLearningDeliveries(long? fundModelNullable, string famType = "SOF")
        {
            var learningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    FundModelNullable = fundModelNullable,
                    LearningDeliveryFAMs = new[]
                    {
                        new TestLearningDeliveryFAM()
                        {
                            LearnDelFAMType = famType,
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
            return learningDeliveries;
        }
    }
}
