using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.QUALENT3
{
    public class QUALENT3_01RuleTests : AbstractRuleTests<QUALENT3_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("QUALENT3_01");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                QUALENT3 = "QUALENT3"
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
            var learningDeliveryHE = new TestLearningDeliveryHE();

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2009, 08, 01)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE();

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData("QUALENT3", 2018)]
        [InlineData(null, 2008)]
        public void ConditionMet_False(string qualent3, int year)
        {
            var learnStartDate = new DateTime(year, 08, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                QUALENT3 = qualent3
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeFalse();
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
                        LearnStartDate = new DateTime(2015, 1, 1),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                        LearnStartDate = new DateTime(2009, 1, 1),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private QUALENT3_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new QUALENT3_01Rule(validationErrorHandler);
        }
    }
}
