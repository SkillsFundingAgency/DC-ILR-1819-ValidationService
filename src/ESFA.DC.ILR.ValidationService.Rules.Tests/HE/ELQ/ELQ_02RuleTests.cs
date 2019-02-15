using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.ELQ;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.ELQ
{
    public class ELQ_02RuleTests : AbstractRuleTests<ELQ_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ELQ_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE { ELQNullable = 1 };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(TypeOfIntegerCodedLookup.ELQ, 1)).Returns(false);

            NewRule(providerDetailsMock.Object).ConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE { ELQNullable = 1 };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(TypeOfIntegerCodedLookup.ELQ, 1)).Returns(true);

            NewRule(providerDetailsMock.Object).ConditionMet(learningDeliveryHe).Should().BeFalse();
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
                       LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            ELQNullable = 2
                        }
                    }
                }
            };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(TypeOfIntegerCodedLookup.ELQ, 2)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(providerDetailsMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            ELQNullable = 2
                        }
                    }
                }
            };

            var providerDetailsMock = new Mock<IProvideLookupDetails>();

            providerDetailsMock.Setup(ds => ds.Contains(TypeOfIntegerCodedLookup.ELQ, 2)).Returns(true);

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
                        FundModel = 35,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            ELQNullable = null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ELQNull()
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
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ELQ, 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);
            validationErrorHandlerMock.Verify();
        }

        private ELQ_02Rule NewRule(IProvideLookupDetails provideLookupDetails = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ELQ_02Rule(provideLookupDetails, validationErrorHandler);
        }
    }
}
