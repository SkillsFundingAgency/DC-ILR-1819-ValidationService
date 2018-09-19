using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_02RuleTests
    {
        [Theory]
        [InlineData("RUI", 1)]
        [InlineData("PMC", 1)]
        public void ConditionMet_True(string type, long? code)
        {
            var rule = NewRule();
            var contactPreferences = SetupContactPreferences(type, code);

            rule.ConditionMet("RUI", 3, contactPreferences).Should().BeTrue();
        }

        [Theory]
        [InlineData("RUI", 100)]
        [InlineData("PMC", 100)]
        public void ConditionMet_False(string type, long? code)
        {
            var rule = NewRule();
            var contactPreferences = SetupContactPreferences(type, code);

            rule.ConditionMet("RUI", 3, contactPreferences).Should().BeFalse();
        }

        [Theory]
        [InlineData("RUI", 3)]
        [InlineData("RUI", 4)]
        [InlineData("RUI", 5)]
        public void ConditionMetNotToBeContacted_True(string type, long? code)
        {
            var rule = NewRule();
            rule.ConditionMetNotToBeContacted(type, code).Should().BeTrue();
        }

        [Theory]
        [InlineData("", 3)]
        [InlineData(null, 4)]
        [InlineData("RUI", 10)]
        [InlineData("RUI", 1)]
        public void ConditionMetNotToBeContacted_False(string type, long? code)
        {
            var rule = NewRule();
            rule.ConditionMetNotToBeContacted(type, code).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void ConditionMetContactRUI_True(long? code)
        {
            var contactPreferences = SetupContactPreferences("RUI", code);
            var rule = NewRule();
            rule.ConditionMetContactRUI(contactPreferences).Should().BeTrue();
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(0)]
        [InlineData(null)]
        public void ConditionMetContactRUI_False(long? code)
        {
            var contactPreferences = SetupContactPreferences("RUI", code);
            var rule = NewRule();
            rule.ConditionMetContactRUI(contactPreferences).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ConditionMetContactPMC_True(long? code)
        {
            var contactPreferences = SetupContactPreferences("PMC", code);

            var rule = NewRule();
            rule.ConditionMetContactPMC(contactPreferences).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(99)]
        public void ConditionMetContactPMC_False(long? code)
        {
            var contactPreferences = SetupContactPreferences("PMC", code);

            var rule = NewRule();
            rule.ConditionMetContactPMC(contactPreferences).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var contactPreferences = SetupContactPreferences("PMC", 1);
            contactPreferences.AddRange(SetupContactPreferences("RUI", 3));

            var learner = new TestLearner()
            {
                ContactPreferences = contactPreferences
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_02", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var contactPreferences = SetupContactPreferences("PMC", 1);
            contactPreferences.AddRange(SetupContactPreferences("RUI", 9));

            var learner = new TestLearner()
            {
                ContactPreferences = contactPreferences
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_02", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private List<TestContactPreference> SetupContactPreferences(string type, long? code)
        {
            var contactPreferences = new List<TestContactPreference>()
            {
                new TestContactPreference()
                {
                    ContPrefCodeNullable = code,
                    ContPrefType = type
                },
                new TestContactPreference()
                {
                    ContPrefCodeNullable = 999,
                    ContPrefType = "XYZ"
                },
            };
            return contactPreferences;
        }

        private ContPrefType_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ContPrefType_02Rule(validationErrorHandler);
        }
    }
}