using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCOLAB;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCOLAB
{
    public class PCOLAB_01RuleTests : AbstractRuleTests<PCOLAB_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCOLAB_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            int? partnerUkprn = 123;
            decimal? pcolab = null;

            NewRule().ConditionMet(learnStartDate, partnerUkprn, pcolab).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var learnStartDate = new DateTime(2013, 07, 31);
            int? partnerUkprn = 123;
            decimal? pcolab = null;

            NewRule().ConditionMet(learnStartDate, partnerUkprn, pcolab).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PartnerUkprnIsNull()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            int? partnerUkprn = null;
            decimal? pcolab = null;

            NewRule().ConditionMet(learnStartDate, partnerUkprn, pcolab).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PCOLABHasValue()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            int? partnerUkprn = 123;
            decimal? pcolab = 1;

            NewRule().ConditionMet(learnStartDate, partnerUkprn, pcolab).Should().BeFalse();
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
                        PartnerUKPRNNullable = 123,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCOLABNullable = null
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
                        PartnerUKPRNNullable = 123,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCOLABNullable = 1
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
            var learnStartDate = new DateTime(2018, 01, 01);
            int? partnerUkprn = 123;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PartnerUKPRN, partnerUkprn)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, partnerUkprn);

            validationErrorHandlerMock.Verify();
        }

        private PCOLAB_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCOLAB_01Rule(validationErrorHandler);
        }
    }
}
