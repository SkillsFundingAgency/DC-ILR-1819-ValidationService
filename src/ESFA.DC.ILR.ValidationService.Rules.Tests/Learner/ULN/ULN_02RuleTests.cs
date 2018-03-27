using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_02RuleTests
    {
        [Fact]
        public void Exclude_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(true);

            var uln_02 = new ULN_02Rule(learningDeliveryFAMQueryServiceMock.Object, null);

            uln_02.Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "SOF", "1")).Returns(false);

            var uln_02 = new ULN_02Rule(learningDeliveryFAMQueryServiceMock.Object, null);

            uln_02.Exclude(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(99)]
        [InlineData(10)]
        public void ConditionMet_True(long fundModel)
        {
            var uln_02 = new ULN_02Rule(null, null);

            uln_02.ConditionMet(fundModel, 9999999999).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Uln()
        {
            var uln_02 = new ULN_02Rule(null, null);

            uln_02.ConditionMet(10, 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var uln_02 = new ULN_02Rule(null, null);

            uln_02.ConditionMet(1, 9999999999).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var messageLearner = new TestLearner()
            {
                ULNNullable = 1,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 2,
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "1")).Returns(false);

            var uln_02 = new ULN_02Rule(learningDeliveryFAMQueryServiceMock.Object, null);

            uln_02.Validate(messageLearner);
        }

        [Fact]
        public void Validate_Errors()
        {
            var messageLearner = new TestLearner()
            {
                ULNNullable = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 10,
                    },
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 99,
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "1")).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_02", null, null, null);
            validationErrorHandlerMock.Setup(handle);

            var uln_02 = new ULN_02Rule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            uln_02.Validate(messageLearner);

            validationErrorHandlerMock.Verify(handle, Times.Exactly(2));
        }
    }
}
