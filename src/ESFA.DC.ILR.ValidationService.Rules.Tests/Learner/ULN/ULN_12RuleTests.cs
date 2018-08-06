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
    public class ULN_12RuleTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(9999999999)]
        public void ConditionMet_True_NullULN(long? uln)
        {
            NewRule().ConditionMet(true, uln).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAM()
        {
            NewRule().ConditionMet(false, 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ULN()
        {
            NewRule().ConditionMet(true, 1).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1,
                LearningDeliveries = new TestLearningDelivery[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
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
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                    }
                }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_12", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private ULN_12Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_12Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
