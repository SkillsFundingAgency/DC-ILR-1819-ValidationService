using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.MSTUFEE
{
    public class MSTUFEE_04RuleTests : AbstractRuleTests<MSTUFEE_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MSTUFEE_04");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void ConditionMet_True(int mstufee)
        {
            var learningDeliveryHe = new TestLearningDeliveryHE
            {
                MSTUFEE = mstufee,
                DOMICILE = "XG"
            };

            NewRule().ConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseMSTUFEE()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE
            {
                MSTUFEE = 0,
                DOMICILE = "XG"
            };

            NewRule().ConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseDOMICILE()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE
            {
                MSTUFEE = 2,
                DOMICILE = "XX"
            };

            NewRule().ConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseNull()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
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
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            MSTUFEE = 2,
                            DOMICILE = "XG"
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
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            MSTUFEE = 2,
                            DOMICILE = "XX"
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, 2)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, "XG")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(2, "XG");

            validationErrorHandlerMock.Verify();
        }

        private MSTUFEE_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new MSTUFEE_04Rule(validationErrorHandler);
        }
    }
}
