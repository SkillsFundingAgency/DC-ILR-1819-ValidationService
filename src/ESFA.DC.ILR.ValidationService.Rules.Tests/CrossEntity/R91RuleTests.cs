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

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.References.SupportedInternship16To19, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfAim.References.SupportedInternship16To19, CompletionState.IsOngoing, "ESF-999999999")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-123456789")]
        public void ConditionMet_True(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
        {
            var testLearningDeliveries = new[]
            {
                new TestLearningDelivery()
                {
                    FundModel = fundModel,
                    LearnAimRef = learnAimRef,
                    CompStatus = compStatus,
                    ConRefNumber = conRefNumber
                },
                new TestLearningDelivery()
                {
                    FundModel = TypeOfFunding.EuropeanSocialFund,
                    LearnAimRef = TypeOfAim.References.ESFLearnerStartandAssessment,
                    CompStatus = CompletionState.HasCompleted,
                    ConRefNumber = "ESF-999999999"
                }
            };

            NewRule().ConditionMet(testLearningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var testLearningDeliveries = new[]
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
            };

            NewRule().ConditionMet(testLearningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
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
                        ConRefNumber = "ESF-999999999"
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, string.Empty)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, CompletionState.HasCompleted)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, string.Empty, CompletionState.HasCompleted);
            validationErrorHandlerMock.Verify();
        }

        public R91Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R91Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
