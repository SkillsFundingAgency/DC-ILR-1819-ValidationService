using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R91RuleTests : AbstractRuleTests<R91Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R91");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.ApprenticeshipsFrom1May2017).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.EuropeanSocialFund).Should().BeTrue();
        }

        [Fact]
        public void LearnAimRefConditionMet_False()
        {
            NewRule().LearnAimRefConditionMet(TypeOfAim.References.SupportedInternship16To19).Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefConditionMet_True()
        {
            NewRule().LearnAimRefConditionMet(TypeOfAim.References.ESFLearnerStartandAssessment).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            NewRule().CompStatusConditionMet(CompletionState.IsOngoing).Should().BeFalse();
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            NewRule().CompStatusConditionMet(CompletionState.HasCompleted).Should().BeTrue();
        }

        [Fact]
        public void ContractReferenceConditionMet_False()
        {
            NewRule().ContractReferenceConditionMet("ZESF0003", new HashSet<string>() { "ZESF0001" }).Should().BeFalse();
        }

        [Fact]
        public void ContractReferenceConditionMet_True()
        {
            NewRule().ContractReferenceConditionMet("ZESF0001", new HashSet<string>() { "ZESF0001" }).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasTemporarilyWithdrawn)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.IndustryPlacement, CompletionState.HasCompleted)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.IndustryPlacement, CompletionState.HasWithdrawn)]
        public void Validate_Error(int fundModel, string learnAimRef, int compStatus)
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearnAimRef = TypeOfAim.References.SupportedInternship16To19,
                        CompStatus = CompletionState.IsOngoing,
                        ConRefNumber = "ESF-123445679"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = "ESF-123445679"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        CompStatus = compStatus,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasTemporarilyWithdrawn,
                        ConRefNumber = "ESF-554466331"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = "ESF-999999999"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NoFundModel()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships,
                        LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                        CompStatus = CompletionState.HasCompleted,
                        ConRefNumber = "ESF-999999999"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, "ESF-123456789")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, CompletionState.HasCompleted)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, "ESF-123456789", CompletionState.HasCompleted);
            validationErrorHandlerMock.Verify();
        }

        public R91Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R91Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
