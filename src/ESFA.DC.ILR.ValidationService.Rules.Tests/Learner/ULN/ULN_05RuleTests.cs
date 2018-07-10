using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_05RuleTests : AbstractRuleTests<ULN_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ULN_05");
        }

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
                ULN = 1
            };

            var ulnDataServiceMock = new Mock<IULNDataService>();

            ulnDataServiceMock.Setup(urds => urds.Exists(1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(ulnDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULN = 1,
            };

            var ulnDataServiceMock = new Mock<IULNDataService>();

            ulnDataServiceMock.Setup(urds => urds.Exists(1)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(ulnDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", (long)1234567890)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1234567890);

            validationErrorHandlerMock.Verify();
        }

        private ULN_05Rule NewRule(IULNDataService ulnDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_05Rule(ulnDataService, validationErrorHandler);
        }
    }
}
