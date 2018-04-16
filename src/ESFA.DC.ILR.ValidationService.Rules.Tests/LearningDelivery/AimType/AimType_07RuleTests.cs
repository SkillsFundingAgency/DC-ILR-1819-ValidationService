using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AimType
{
    public class AimType_07RuleTests : AbstractRuleTests<AimType_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AimType_07");
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet(24, 5, new DateTime(2018, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            NewRule().ConditionMet(23, 4, new DateTime(2018, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            NewRule().ConditionMet(23, 4, new DateTime(2016, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(23, 5, new DateTime(2018, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "SOF", "105")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "SOF", "105")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 23,
                AimType = 5,
                LearnStartDate = new DateTime(2018, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "SOF", "105")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 24,
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AimType_07Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AimType_07Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
