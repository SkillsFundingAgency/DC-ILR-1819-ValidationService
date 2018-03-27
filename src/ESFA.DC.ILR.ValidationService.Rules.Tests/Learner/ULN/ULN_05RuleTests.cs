using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.ExternalData.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_05RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1, false).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Uln9s()
        {
            NewRule().ConditionMet(9999999999, false).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_UlnExists()
        {
            NewRule().ConditionMet(1, true).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1
            };

            var ulnReferenceDataServiceMock = new Mock<IULNReferenceDataService>();

            ulnReferenceDataServiceMock.Setup(urds => urds.Exists(1)).Returns(true);

            var rule = NewRule(ulnReferenceDataServiceMock.Object);

            rule.Validate(learner);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1,
            };

            var ulnReferenceDataServiceMock = new Mock<IULNReferenceDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            ulnReferenceDataServiceMock.Setup(urds => urds.Exists(1)).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_05", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(ulnReferenceDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private ULN_05Rule NewRule(IULNReferenceDataService uLNReferenceDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_05Rule(uLNReferenceDataService, validationErrorHandler);
        }
    }
}
