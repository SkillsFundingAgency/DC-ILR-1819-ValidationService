using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FUNDLEV
{
    public class MODESTUD_03RuleTests : AbstractRuleTests<MODESTUD_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MODESTUD_03Rule");
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2009, 08, 02)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2009, 07, 31)).Should().BeFalse();
        }

        [Theory]
        [InlineData(4, 1, true)]
        [InlineData(5, 1, true)]
        [InlineData(4, 3, false)]
        [InlineData(5, 3, false)]
        public void LearningDeliveryHeConditionMeetsExpectation(int specFee, int modeStud, bool expectation)
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                SPECFEE = specFee,
                MODESTUD = modeStud
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHe).Should().Be(expectation);
        }

        [Theory]
        [InlineData("08/01/2009", 4, 3, false)]
        [InlineData("08/02/2009", 4, 3, false)]
        [InlineData("08/01/2009", 4, 1, true)]
        [InlineData("08/02/2009", 4, 2, true)]
        [InlineData("07/31/2009", 4, 3, false)]
        [InlineData("08/01/2009", 5, 3, false)]
        [InlineData("08/02/2009", 5, 3, false)]
        [InlineData("08/01/2009", 5, 1, true)]
        [InlineData("08/02/2009", 5, 2, true)]
        [InlineData("07/31/2009", 5, 3, false)]
        public void ConditionMeetsExpectation(string strLearnStartDate, int specFee, int modeStud, bool expectation)
        {
            var learnStartDate = DateTime.Parse(strLearnStartDate);
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                SPECFEE = specFee,
                MODESTUD = modeStud
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHe).Should().Be(expectation);
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
                        LearnStartDate = new DateTime(2009, 08, 02),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SPECFEE = 4,
                            MODESTUD = 1
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
                        LearnStartDate = new DateTime(2009, 08, 02),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            SPECFEE = 4,
                            MODESTUD = 3
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
        public void Validate_NoError_NullLearningDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = null
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 1, 1);

            validationErrorHandlerMock.Verify();
        }

        private MODESTUD_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new MODESTUD_03Rule(validationErrorHandler);
        }
    }
}
