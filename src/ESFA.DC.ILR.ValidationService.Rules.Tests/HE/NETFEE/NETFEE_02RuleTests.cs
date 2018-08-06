using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.NETFEE
{
    public class NETFEE_02RuleTests : AbstractRuleTests<NETFEE_02RuleTests>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("NETFEE_02");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                NETFEENullable = 1000
            };
            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False_NullEntity()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                NETFEENullable = 9100
            };
            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData(7000)]
        public void ConditionMet_False(int? netfee)
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                NETFEENullable = netfee
            };
            NewRule().ConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            NETFEENullable = 9100
                        }
                    }
                }
            };

            using (var validationErrorhandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorhandlerMock.Object).Validate(learner);
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
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            NETFEENullable = 7000
                        }
                    }
                }
            };
            using (var validateErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validateErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("NETFEE", 10200)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(10200);

            validationErrorHandlerMock.Verify();
        }

        private NETFEE_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new NETFEE_02Rule(validationErrorHandler);
        }
    }
}
