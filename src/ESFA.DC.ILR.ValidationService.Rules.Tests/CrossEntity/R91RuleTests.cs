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

        [Theory]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.IsOngoing, "ESF-987654321")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-999999999")]
        public void ConRefConditionMet_False(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
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
                    FundModel = TypeOfFunding.CommunityLearning,
                    LearnAimRef = TypeOfAim.References.SupportedInternship16To19,
                    CompStatus = CompletionState.HasWithdrawn,
                    ConRefNumber = "ESF-123456789"
                }
            };

            NewRule().ConRefConditionMet(testLearningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-999999999")]
        public void ConRefConditionMet_FalseOnMatching(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
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

            NewRule().ConRefConditionMet(testLearningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-999999999")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.IsOngoing, "ESF-999999999")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.HasCompleted, "ESF-999999999")]
        public void ConRefConditionMet_True(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
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

            NewRule().ConRefConditionMet(testLearningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.IsOngoing, "ESF-987654321")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.SupportedInternship16To19, CompletionState.HasCompleted, "ESF-999999999")]
        [InlineData(TypeOfFunding.Other16To19, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-999999999")]
        public void ConditionMet_False(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
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
                    FundModel = TypeOfFunding.CommunityLearning,
                    LearnAimRef = TypeOfAim.References.SupportedInternship16To19,
                    CompStatus = CompletionState.HasWithdrawn,
                    ConRefNumber = "ESF-123456789"
                }
            };

            NewRule().ConditionMet(fundModel, testLearningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-999999999")]
        public void ConditionMet_FalseOnMatching(int fundModel, string learnAimRef, int compStatus, string conRefNumber)
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

            NewRule().ConditionMet(fundModel, testLearningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.HasCompleted, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.IsOngoing, "ESF-123456789")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.ESFLearnerStartandAssessment, CompletionState.IsOngoing, "ESF-999999999")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.IsOngoing, "ESF-999999999")]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfAim.References.WorkExperience, CompletionState.HasCompleted, "ESF-999999999")]
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

            NewRule().ConditionMet(fundModel, testLearningDeliveries).Should().BeTrue();
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
