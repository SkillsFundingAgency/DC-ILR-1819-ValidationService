using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SPECFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SPECFEE
{
    public class SPECFEE_02RuleTests : AbstractRuleTests<SPECFEE_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SPECFEE_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var modestud = 2;
            var specfee = 0;

            NewRule().ConditionMet(learnStartDate, modestud, specfee).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThanFirstAugust2009()
        {
            var learnStartDate = new DateTime(2008, 01, 01);
            var modestud = 2;
            var specfee = 0;

            NewRule().ConditionMet(learnStartDate, modestud, specfee).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ModestudMismatch()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var modestud = 1;
            var specfee = 0;

            NewRule().ConditionMet(learnStartDate, modestud, specfee).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_SpecFeeIsSandwichPlacement()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var modestud = 2;
            var specfee = 1;

            NewRule().ConditionMet(learnStartDate, modestud, specfee).Should().BeFalse();
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
                            MODESTUD = 2,
                            SPECFEE = 0
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
                            MODESTUD = 2,
                            SPECFEE = 1
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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MODESTUD, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, 2)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01), 1, 2);

            validationErrorHandlerMock.Verify();
        }

        private SPECFEE_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new SPECFEE_02Rule(validationErrorHandler);
        }
    }
}
