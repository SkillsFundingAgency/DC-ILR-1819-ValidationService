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
    public class ContPrefType_07RuleTests : AbstractRuleTests<ContPrefType_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ContPrefType_07");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var RUI = ContactPreference.Types.RestrictedUserInteraction;
            var PMC = ContactPreference.Types.PreferredMethodOfContact;

            var contactPreferences = new TestContactPreference[]
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 3, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 4, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 1, ContPrefType = RUI }
            };

            NewRule().ConditionMet(contactPreferences).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var RUI = ContactPreference.Types.RestrictedUserInteraction;
            var PMC = ContactPreference.Types.PreferredMethodOfContact;

            var contactPreferences = new TestContactPreference[]
            {
                new TestContactPreference { ContPrefCode = 1, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 2, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 3, ContPrefType = PMC },
                new TestContactPreference { ContPrefCode = 1, ContPrefType = RUI },
            };

            NewRule().ConditionMet(contactPreferences).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var RUI = ContactPreference.Types.RestrictedUserInteraction;
            var PMC = ContactPreference.Types.PreferredMethodOfContact;

            var learner = new TestLearner
            {
                ContactPreferences = new List<TestContactPreference>()
                {
                    new TestContactPreference { ContPrefCode = 1, ContPrefType = PMC },
                    new TestContactPreference { ContPrefCode = 2, ContPrefType = PMC },
                    new TestContactPreference { ContPrefCode = 3, ContPrefType = PMC },
                    new TestContactPreference { ContPrefCode = 4, ContPrefType = PMC },
                    new TestContactPreference { ContPrefCode = 1, ContPrefType = RUI }
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
            var RUI = ContactPreference.Types.RestrictedUserInteraction;
            var PMC = ContactPreference.Types.PreferredMethodOfContact;

            var learner = new TestLearner
            {
                ContactPreferences = new List<TestContactPreference>()
                {
                    new TestContactPreference { ContPrefCode = 1, ContPrefType = RUI }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoErrorNullContactPreferences()
        {
            var learner = new TestLearner();

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

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(ContactPreference.Types.PreferredMethodOfContact);

            validationErrorHandlerMock.Verify();
        }

        private ContPrefType_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ContPrefType_07Rule(validationErrorHandler);
        }
    }
}
