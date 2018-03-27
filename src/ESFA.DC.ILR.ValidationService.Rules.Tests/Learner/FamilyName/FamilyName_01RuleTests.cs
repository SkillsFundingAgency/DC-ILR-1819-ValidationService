using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.FamilyName;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.FamilyName
{
    public class FamilyName_01RuleTests
    {
        [Fact]
        public void Exclude_True_FundModel10()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModelNullable = 10
            };

            NewRule().Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_True_FundModel99()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModelNullable = 99,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "108")).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModelNullable = 2
            };

            NewRule().Exclude(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_Null()
        {
            NewRule().ConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Whitespace()
        {
            NewRule().ConditionMet("    ").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet("Not Null or White Space").Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                FamilyName = null
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("FamilyName_01", null, null, null);

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
                FamilyName = "Not Null"
            };

            NewRule().Validate(learner);
        }

        [Fact]
        public void Validate_NoErrors_AllExcluded()
        {
            var learner = new TestLearner()
            {
                FamilyName = null,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 10
                    }
                }
            };

            NewRule().Validate(learner);
        }

        private FamilyName_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FamilyName_01Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
