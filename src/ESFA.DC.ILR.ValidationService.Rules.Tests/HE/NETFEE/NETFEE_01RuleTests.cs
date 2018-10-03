using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.NETFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.NETFEE
{
    public class NETFEE_01RuleTests : AbstractRuleTests<NETFEE_01RuleTests>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("NETFEE_01");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                SSN = "2002", NETFEENullable = 500
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
                SSN = null, NETFEENullable = null
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2012, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2008, 01, 01)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2012, 08, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE();

            NewRule().ConditionMet(learnStartDate, learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData("SSN", 500, 2012)]
        [InlineData(null, null, 2008)]
        [InlineData(null, 600, 2012)]
        [InlineData("300", null, 2012)]
        public void ConditionMet_False(string ssn, int? netfee, int year)
        {
            var learnStartDate = new DateTime(year, 08, 01);
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                SSN = ssn,
                NETFEENullable = netfee
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
                        LearnStartDate = new DateTime(2012, 08, 1),
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
                        LearnStartDate = new DateTime(2011, 01, 1),
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2018")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private NETFEE_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new NETFEE_01Rule(validationErrorHandler);
        }
    }
}
