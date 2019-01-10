using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ContPrefType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ContPrefType
{
    public class ContPrefType_05RuleTests : AbstractRuleTests<ContPrefType_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ContPrefType_05");
        }

        [Fact]
        public void HasAnyPMUContactPreferenceForCodes_True()
        {
            var contPrefCodes = new[] { 1, 2, 3 };

            var contactPreferences = new List<TestContactPreference>
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 3, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
            };

            NewRule().HasAnyPMUContactPreferenceForCodes(contactPreferences, contPrefCodes).Should().BeTrue();
        }

        [Fact]
        public void HasAnyPMUContactPreferenceForCodes_False_NoPMUMatch()
        {
            var contPrefCodes = new[] { 1, 2, 3 };

            var contactPreferences = new List<TestContactPreference>
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.RestrictedUserInteraction },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = ContactPreference.Types.RestrictedUserInteraction },
                new TestContactPreference { ContPrefCode = 3, ContPrefType = ContactPreference.Types.RestrictedUserInteraction },
            };

            NewRule().HasAnyPMUContactPreferenceForCodes(contactPreferences, contPrefCodes).Should().BeFalse();
        }

        [Fact]
        public void HasAnyPMUContactPreferenceForCodes_False_NoCodeMatch()
        {
            var contPrefCodes = new[] { 4, 5, 6 };

            var contactPreferences = new List<TestContactPreference>
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 3, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
            };

            NewRule().HasAnyPMUContactPreferenceForCodes(contactPreferences, contPrefCodes).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var contactPreferences = new List<TestContactPreference>
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 6, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
            };

            NewRule().ConditionMet(contactPreferences).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var contactPreferences = new List<TestContactPreference>
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
            };

            NewRule().ConditionMet(contactPreferences).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                ContactPreferences = new List<TestContactPreference>()
                {
                    new TestContactPreference { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                    new TestContactPreference { ContPrefCode = 6, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner
            {
                ContactPreferences = new List<TestContactPreference>()
                {
                    new TestContactPreference
                        { ContPrefCode = 1, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                    new TestContactPreference
                        { ContPrefCode = 2, ContPrefType = ContactPreference.Types.PreferredMethodOfContact },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ContPrefType", ContactPreference.Types.PreferredMethodOfContact)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ContPrefCode", "1, 2")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(ContactPreference.Types.PreferredMethodOfContact, "1, 2");

            validationErrorHandlerMock.Verify();
        }

        private ContPrefType_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ContPrefType_05Rule(validationErrorHandler);
        }
    }
}
