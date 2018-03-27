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
    public class LLDDHealthProb_06RuleTests
    {
        [Fact]
        public void ConditionMet_True_NullLLDHealthAndProblems()
        {
            var rule = NewRule();
            rule.ConditionMet(1, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Empty()
        {
            var rule = NewRule();
            rule.ConditionMet(1,  new List<ILLDDAndHealthProblem>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();
            var llddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem() { }
            };
            rule.ConditionMet(2, llddAndHealthProblems).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2)]
        public void ConditionMet_False(long? llddHealthProblem)
        {
            var rule = NewRule();
            var llddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem() { }
            };
            rule.ConditionMet(llddHealthProblem, llddAndHealthProblems).Should().BeFalse();
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

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);
            rule.ExcludeConditionFamValueMet(99, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ExcludeFundMode99Met_True()
        {
            var learningDeliveryFams = SetupLearningDeliveries(It.IsAny<long>()).First().LearningDeliveryFAMs;

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var rule = NewRule(null, null, learningDeliveryFAMQueryServiceMock.Object);

            rule.ExcludeConditionFamValueMet(99, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void Exclude_LearningDeliveriesNull_False()
        {
            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(It.IsAny<DateTime>());

            var rule = NewRule(null, dd06Mock.Object);
            rule.Exclude(null, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("1996-01-01", null)]
        [InlineData(null, "2017-10-10")]
        [InlineData("1992-10-11", "2017-10-10")]
        public void Exclude_False_DateofBirth(string dateOfBirth, string minimumLearnStart)
        {
            var dateofBirthDate = string.IsNullOrEmpty(dateOfBirth) ? (DateTime?)null : DateTime.Parse(dateOfBirth);
            var minimumLearnStartDate = string.IsNullOrEmpty(minimumLearnStart) ? (DateTime?)null : DateTime.Parse(minimumLearnStart);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(24);

            var rule = NewRule(null, null, null, dateTimeQueryServiceMock.Object);

            rule.ExcludeConditionDateOfBirthMet(dateofBirthDate, minimumLearnStartDate).Should().BeFalse();
        }

        [Fact]
        public void Exclude_True_DateOfBirth()
        {
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(new DateTime(1992, 10, 10), new DateTime(2017, 10, 10))).Returns(25);
            var rule = NewRule(null, null, null, dateTimeQueryServiceMock.Object);

            rule.ExcludeConditionDateOfBirthMet(new DateTime(1992, 10, 10), new DateTime(2017, 10, 10)).Should().BeTrue();
        }

        [Fact]
        public void Exclude_True()
        {
            var learningDeliveries = SetupLearningDeliveries(99);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2017, 10, 10));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(25);

            var rule = NewRule(null, dd06Mock.Object, learningDeliveryFAMQueryServiceMock.Object, dateTimeQueryServiceMock.Object);

            rule.Exclude(learningDeliveries, new DateTime(1992, 10, 10)).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDeliveries = SetupLearningDeliveries(9999);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "108")).Returns(true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2018, 10, 10));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x =>
                x.YearsBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(24);

            var rule = NewRule(null, dd06Mock.Object, learningDeliveryFAMQueryServiceMock.Object, dateTimeQueryServiceMock.Object);

            rule.Exclude(learningDeliveries, It.IsAny<DateTime>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var validationErrorHandlerMock = SetupLearnerForValidate(new DateTime(1992, 11, 10), null);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_06", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupLearnerForValidate(new DateTime(1992, 10, 10), new List<ILLDDAndHealthProblem>() { new TestLLDDAndHealthProblem() });
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDHealthProb_06", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Mock<IValidationErrorHandler> SetupLearnerForValidate(DateTime dateOfBirth, IReadOnlyCollection<ILLDDAndHealthProblem> lldHealthProblems)
        {
            var learner = new TestLearner()
            {
                LLDDHealthProbNullable = 1,
                LearningDeliveries = SetupLearningDeliveries(100, "XYZ"),
                DateOfBirthNullable = dateOfBirth,
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

        private LLDDHealthProb_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IDD06 dd06 = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LLDDHealthProb_06Rule(validationErrorHandler, dd06, learningDeliveryFAMQueryService, dateTimeQueryService);
        }
    }
}
