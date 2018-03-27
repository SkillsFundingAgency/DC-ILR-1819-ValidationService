using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_01RuleTests
    {
        [Fact]
        public void Exclude_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "034")).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "034")).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(70)]
        [InlineData(81)]
        [InlineData(82)]
        public void ConditionMet_True(long fundModel)
        {
            var rule = NewRule();

            rule.ConditionMet(fundModel, null);
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth()
        {
            var rule = NewRule();

            rule.ConditionMet(25, new DateTime(1988, 12, 25)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_Null()
        {
            var rule = NewRule();

            rule.ConditionMet(null, new DateTime(1988, 12, 25)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var rule = NewRule();

            rule.ConditionMet(1, new DateTime(1988, 12, 25)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("DateOfBirth_01", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1988, 12, 25),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 25
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
        }

        private DateOfBirth_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_01Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
