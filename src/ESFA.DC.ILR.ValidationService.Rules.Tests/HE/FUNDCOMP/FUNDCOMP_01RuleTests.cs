using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDCOMP;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FUNDCOMP
{
    public class FUNDCOMP_01RuleTests : AbstractRuleTests<FUNDCOMP_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FUNDCOMP_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundComp = 1;
            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.FundComp, 1)).Returns(false);
            NewRule(providerDetailsMock.Object).ConditionMet(fundComp).Should().BeTrue();
        }

        [Fact] public void ConditionMet_False()
        {
            var fundComp = 1;
            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.FundComp, 1)).Returns(true);
            NewRule(providerDetailsMock.Object).ConditionMet(fundComp).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fundComp = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                       LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = fundComp
                        }
                    }
                }
            };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.FundComp, fundComp)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(providerDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var fundComp = 1;
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    },
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDCOMP = fundComp
                        }
                    }
                }
            };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();
            providerDetailsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.FundComp, fundComp)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(providerDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveryHE()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "123"
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FUNDCOMP, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);
            validationErrorHandlerMock.Verify();
        }

        private FUNDCOMP_01Rule NewRule(IProvideLookupDetails provideLookupDetails = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FUNDCOMP_01Rule(provideLookupDetails, validationErrorHandler);
        }
    }
}
