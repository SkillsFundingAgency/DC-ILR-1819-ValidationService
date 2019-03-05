using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OtherFundAdj;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OtherFundAdj
{
    public class OtherFundAdj_01RuleTests : AbstractRuleTests<OtherFundAdj_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OtherFundAdj_01");
        }

        [Fact]
        public void OtherFundAdjConditionMet_True()
        {
            NewRule().OtherFundAdjConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void OtherFundAdjConditionMet_False()
        {
            NewRule().OtherFundAdjConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 25;
            int? otherLearnFundAdj = 100;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.OtherFundAdjConditionMet(otherLearnFundAdj)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);

            ruleMock.Object.ConditionMet(otherLearnFundAdj, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var fundModel = 25;
            int? otherLearnFundAdj = 100;

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.OtherFundAdjConditionMet(otherLearnFundAdj)).Returns(true);
            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);

            ruleMock.Object.ConditionMet(otherLearnFundAdj, fundModel).Should().BeFalse();
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
                        FundModel = 25,
                        OtherFundAdjNullable = 100
                    }
                }
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
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25
                    }
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 25)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OtherFundAdj", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(25, 1);

            validationErrorHandlerMock.Verify();
        }

        private OtherFundAdj_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OtherFundAdj_01Rule(validationErrorHandler);
        }
    }
}
