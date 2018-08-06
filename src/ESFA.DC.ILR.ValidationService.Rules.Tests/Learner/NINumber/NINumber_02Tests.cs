using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.NINumber
{
    public class NINumber_02Tests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void ConditionMet_True(string niNumber)
        {
            var rule = new NINumber_02Rule(null, null);
            rule.ConditionMet(niNumber, true).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NonNullNiNumber()
        {
            var rule = new NINumber_02Rule(null, null);
            rule.ConditionMet("NINUMBER", true).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NotApplicableFAM()
        {
            var rule = new NINumber_02Rule(null, null);
            rule.ConditionMet(null, false).Should().BeFalse();
        }

        [Theory]
        [InlineData(" ", "ACT", "2")]
        [InlineData(null, "ACT", "2")]
        [InlineData(null, "XYZ", "1")]
        [InlineData("AZ123456C", "ACT", "1")]
        public void Validate_NoErrors(string niNumber, string famCode, string famType)
        {
            var learningDeliveries = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learner = new TestLearner()
            {
                NINumber = niNumber,
                LearningDeliveries = new TestLearningDelivery[] { learningDeliveries }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("NINumber_02", null, null, null);

            var famQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            famQueryService.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                                        It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), famType, famCode))
                                        .Returns(famType == "ACT" && famCode == "1");

            var rule = new NINumber_02Rule(validationErrorHandlerMock.Object, famQueryService.Object);

            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_Error(string niNumber)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learner = new TestLearner()
            {
                NINumber = niNumber,
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("NINumber_02", null, null, null);

            var famQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            famQueryService.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(
                                        It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1"))
                                        .Returns(true);

            var rule = new NINumber_02Rule(validationErrorHandlerMock.Object, famQueryService.Object);

            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }
    }
}
