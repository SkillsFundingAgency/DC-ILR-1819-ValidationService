using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_03RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var priorAttainReferenceDataServiceMock = new Mock<IPriorAttainInternalDataService>();

            priorAttainReferenceDataServiceMock.Setup(rds => rds.Exists(1)).Returns(false);

            var rule = new PriorAttain_03Rule(priorAttainReferenceDataServiceMock.Object, null);
            rule.ConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var priorAttainReferenceDataServiceMock = new Mock<IPriorAttainInternalDataService>();

            priorAttainReferenceDataServiceMock.Setup(rds => rds.Exists(1)).Returns(true);

            var rule = new PriorAttain_03Rule(priorAttainReferenceDataServiceMock.Object, null);
            rule.ConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                PriorAttainNullable = 100
            };

            var priorAttainReferenceDataServiceMock = new Mock<IPriorAttainInternalDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            priorAttainReferenceDataServiceMock.Setup(rds => rds.Exists(100)).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_03", null, null, null);

            var rule = new PriorAttain_03Rule(priorAttainReferenceDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                PriorAttainNullable = 11
            };

            var priorAttainReferenceDataServiceMock = new Mock<IPriorAttainInternalDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            priorAttainReferenceDataServiceMock.Setup(rds => rds.Exists(11)).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PriorAttain_03", null, null, null);

            var rule = new PriorAttain_03Rule(priorAttainReferenceDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private PriorAttain_03Rule NewRule(IPriorAttainInternalDataService priorAttainReferenceDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_03Rule(priorAttainReferenceDataService, validationErrorHandler);
        }
    }
}