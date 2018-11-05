using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_24RuleTests : AbstractRuleTests<DateOfBirth_24Rule>
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1234, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ULN_Temporary()
        {
            NewRule().ConditionMet(9999999999, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth()
        {
            NewRule().ConditionMet(1234, new DateTime(1990, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                ULN = 1234
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                ULN = 1234,
                DateOfBirthNullable = new DateTime(2000, 01, 01)
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private DateOfBirth_24Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_24Rule(validationErrorHandler);
        }
    }
}
