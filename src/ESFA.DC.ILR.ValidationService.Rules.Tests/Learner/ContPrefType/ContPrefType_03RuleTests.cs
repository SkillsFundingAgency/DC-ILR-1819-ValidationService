using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_03RuleTests
    {
        [Theory]
        [InlineData("RUI", 1, "2100-01-01")]
        [InlineData("PMC", 1, "2100-01-01")]
        [InlineData("RUI", 3, "2014-12-31")]
        public void ConditionMet_True(string type, long? code, string validTo)
        {
            var validToDate = string.IsNullOrEmpty(validTo) ? (DateTime?)null : DateTime.Parse(validTo);

            var contactPreferencesServiceMock = GetContactPreferencesServiceMock(false, true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(validToDate);

            var rule = NewRule(null, contactPreferencesServiceMock.Object, dd06Mock.Object);
            rule.ConditionMet(type, code, validToDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 1, "2013-01-01")]
        [InlineData(null, null, "2013-01-01")]
        [InlineData("RUI", null, "2013-01-01")]
        [InlineData("RUI", null, null)]
        public void ConditionMet_False(string type, long? code, string validTo)
        {
            var validToDate = string.IsNullOrEmpty(validTo) ? (DateTime?)null : DateTime.Parse(validTo);

            var contactPreferencesServiceMock = GetContactPreferencesServiceMock(false, true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(validToDate);

            var rule = NewRule(null, contactPreferencesServiceMock.Object, dd06Mock.Object);
            rule.ConditionMet(type, code, validToDate).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var contactPreferences = SetupContactPreferences("PMC", 1);

            var learner = new TestLearner()
            {
                ContactPreferences = contactPreferences
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_03", null, null, null);

            var contactPreferencesServiceMock = new Mock<IContactPreferenceInternalDataService>();
            contactPreferencesServiceMock.Setup(x =>
                x.TypeForCodeExist(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<DateTime>())).Returns(false);

            contactPreferencesServiceMock.Setup(x => x.TypeExists("PMC")).Returns(true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2100, 10, 10));

            var rule = NewRule(validationErrorHandlerMock.Object, contactPreferencesServiceMock.Object, dd06Mock.Object);

            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var contactPreferences = SetupContactPreferences("PMC", 1);

            var learner = new TestLearner()
            {
                ContactPreferences = contactPreferences
            };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ContPrefType_03", null, null, null);

            var contactPreferencesServiceMock = GetContactPreferencesServiceMock(true, true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2010, 10, 10));

            var rule = NewRule(validationErrorHandlerMock.Object, contactPreferencesServiceMock.Object, dd06Mock.Object);

            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Mock<IContactPreferenceInternalDataService> GetContactPreferencesServiceMock(bool typeForCodeExistResult, bool typeExistsResult)
        {
            var contactPreferencesServiceMock = new Mock<IContactPreferenceInternalDataService>();
            contactPreferencesServiceMock.Setup(x =>
                x.TypeForCodeExist(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<DateTime>())).Returns(typeForCodeExistResult);
            contactPreferencesServiceMock.Setup(x => x.TypeExists(It.IsAny<string>())).Returns(typeExistsResult);

            return contactPreferencesServiceMock;
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

        private ContPrefType_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IContactPreferenceInternalDataService contactPreferenceDataService = null, IDD06 dd06 = null)
        {
            return new ContPrefType_03Rule(validationErrorHandler, contactPreferenceDataService, dd06);
        }
    }
}