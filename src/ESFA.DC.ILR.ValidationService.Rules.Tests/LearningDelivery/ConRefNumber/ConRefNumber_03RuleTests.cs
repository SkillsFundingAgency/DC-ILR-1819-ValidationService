using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ConRefNumber
{
    public class ConRefNumber_03RuleTests : AbstractRuleTests<ConRefNumber_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ConRefNumber_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ConRefNumber()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(10).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(11).Should().BeFalse();
        }

        [Fact]
        public void ConRefNumberConditionMet_True()
        {
            NewRule().ConRefNumberConditionMet("abc").Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        public void ConRefNumberConditionMet_False_Null(string conRefNumber)
        {
            NewRule().ConRefNumberConditionMet(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        ConRefNumber = "abc",
                    }
                }
            };

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                    }
                }
            };

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ConRefNumber", "abc")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, "abc");

            validationErrorHandlerMock.Verify();
        }

        private ConRefNumber_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ConRefNumber_03Rule(validationErrorHandler);
        }
    }
}
