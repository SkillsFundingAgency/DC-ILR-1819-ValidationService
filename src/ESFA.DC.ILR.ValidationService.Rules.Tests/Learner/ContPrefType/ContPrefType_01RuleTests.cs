using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_01RuleTests
    {
        [Theory]
        [InlineData("PMC", 10)]
        [InlineData("RUI", 99)]
        [InlineData("XXXX", 100)]
        public void ConditionMet_True(string type, long? code)
        {
            var contactPreferenceService = new Mock<IContactPreferenceInternalDataService>();
            contactPreferenceService.Setup(x => x.CodeExists(code)).Returns(false);

            var rule = NewRule(null, contactPreferenceService.Object);
            rule.ConditionMet(type, code).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData("", 1)]
        [InlineData(" ", 1)]
        [InlineData("PMC", 1)]
        [InlineData("RUI", 1)]
        [InlineData("RUI", 2)]
        [InlineData("RUI", 3)]
        [InlineData("RUI", 4)]
        [InlineData("RUI", 5)]
        [InlineData("XXXX", 1)]
        public void ConditionMet_False(string type, long? code)
        {
            var contactPreferenceService = new Mock<IContactPreferenceInternalDataService>();
            contactPreferenceService.Setup(x => x.CodeExists(code)).Returns(true);

            var rule = NewRule(null, contactPreferenceService.Object);
            rule.ConditionMet(type, code).Should().BeFalse();
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                ContactPreferences = new List<IContactPreference>()
                {
                    new TestContactPreference()
                    {
                        ContPrefType = "PMC",
                        ContPrefCodeNullable = 1
                    }
                }
            };

            var contactPreferenceService = new Mock<IContactPreferenceInternalDataService>();
            contactPreferenceService.Setup(x => x.CodeExists(1)).Returns(true);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object, contactPreferenceService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                ContactPreferences = new List<IContactPreference>()
                {
                    new TestContactPreference()
                    {
                        ContPrefType = "XXXX",
                        ContPrefCodeNullable = 1
                    }
                }
            };

            var contactPreferenceService = new Mock<IContactPreferenceInternalDataService>();
            contactPreferenceService.Setup(x => x.CodeExists(1)).Returns(false);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_01", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object, contactPreferenceService.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private ContPrefType_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IContactPreferenceInternalDataService contactPreferenceDataService = null)
        {
            return new ContPrefType_01Rule(validationErrorHandler, contactPreferenceDataService);
        }
    }
}