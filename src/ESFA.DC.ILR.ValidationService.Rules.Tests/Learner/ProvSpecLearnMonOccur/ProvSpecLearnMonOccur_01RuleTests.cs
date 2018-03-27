using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ProvSpecLearnMonOccur;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ProvSpecLearnMonOccur
{
    public class ProvSpecLearnMonOccur_01RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            for (var letter = 'C'; letter <= 'Z'; letter++)
            {
                rule.ConditionMet(letter.ToString()).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ConditionMet_False(string provSpecLearnMonOccur)
        {
            var rule = NewRule();
            rule.ConditionMet(provSpecLearnMonOccur).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "X"
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ProvSpecLearnMonOccur_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True_Null()
        {
            var learner = new TestLearner()
            {
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ProvSpecLearnMonOccur_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "A"
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ProvSpecLearnMonOccur_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private ProvSpecLearnMonOccur_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProvSpecLearnMonOccur_01Rule(validationErrorHandler);
        }
    }
}