using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SOC2000;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SOC2000
{
    public class SOC2000RuleTests : AbstractRuleTests<SOC2000_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SOC2000_02");
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learnStartDate = new DateTime(2013, 07, 31);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData("XF")]
        [InlineData("XG")]
        [InlineData("XH")]
        [InlineData("XI")]
        [InlineData("XK")]
        public void LearningDeliveryHEConditionMet_True(string domicile)
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = domicile
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseNull()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseUCASAppId()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                DOMICILE = "XF"
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseDomicile()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = "DEF"
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_FalseSOC2000()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = "XF",
                SOC2000Nullable = 1
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = "XF"
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseLearnStartDate()
        {
            var learnStartDate = new DateTime(2013, 07, 31);
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = "XF"
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLearningDeliveryHE()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                UCASAPPID = "ABC",
                DOMICILE = "XF",
                SOC2000Nullable = 1
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = "ABC",
                            DOMICILE = "XF"
                        }
                    }
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
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = "ABC",
                            DOMICILE = "XF",
                            SOC2000Nullable = 1
                        }
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, "XF")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.UCASAPPID, "ABC")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01), "XF", "ABC");

            validationErrorHandlerMock.Verify();
        }

        private SOC2000_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new SOC2000_02Rule(validationErrorHandler);
        }
    }
}
