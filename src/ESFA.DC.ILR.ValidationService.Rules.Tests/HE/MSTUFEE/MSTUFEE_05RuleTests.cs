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
    public class MSTUFEE_05RuleTests : AbstractRuleTests<MSTUFEE_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MSTUFEE_05");
        }

        [Theory]
        [InlineData(3, "XF", true)]
        [InlineData(3, "XI", true)]
        [InlineData(4, "XF", true)]
        [InlineData(4, "XI", true)]
        [InlineData(3, "xF", true)]
        [InlineData(3, "Xi", true)]
        [InlineData(4, "xF", true)]
        [InlineData(4, "xi", true)]
        [InlineData(2, "XF", false)]
        [InlineData(2, "XI", false)]
        [InlineData(0, "XI", false)]
        [InlineData(0, "XF", false)]
        [InlineData(2, "", false)]
        [InlineData(3, null, false)]
        public void ConditionMetMeetsExpectation(int mstuFee, string domicile, bool expectation)
        {
            var learningDeliveryHe = new TestLearningDeliveryHE
            {
                MSTUFEE = mstuFee,
                DOMICILE = domicile
            };

            NewRule().ConditionMet(learningDeliveryHe).Should().Be(expectation);
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
                        FundModel = 35,
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            MSTUFEE = 3,
                            DOMICILE = "XI"
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
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            MSTUFEE = 5,
                            DOMICILE = "XI"
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
        public void Validate_NoLearningDeliveries_NoError()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoLearningDeliveryHE_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, 3)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, "XF")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(3, "XF");

            validationErrorHandlerMock.Verify();
        }

        private MSTUFEE_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new MSTUFEE_05Rule(validationErrorHandler);
        }
    }
}
