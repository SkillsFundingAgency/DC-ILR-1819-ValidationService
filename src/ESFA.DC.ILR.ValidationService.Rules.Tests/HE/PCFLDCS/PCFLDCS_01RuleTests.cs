using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCFLDCS
{
    public class PCFLDCS_01RuleTests : AbstractRuleTests<PCFLDCS_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCFLDCS_01");
        }

        [Fact]
        public void LDCSConditionMet_True()
        {
            decimal? pcfldcs = 40.5m;
            decimal? pcsldcs = 32.5m;
            decimal? pctldcs = 32.5m;

            NewRule().LDCSConditionMet(pcfldcs, pcsldcs, pctldcs).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("100", null, null)]
        [InlineData(null, "100", null)]
        [InlineData(null, null, "100")]
        [InlineData("35", "32.5", "32.5")]
        public void LDCSConditionMet_False(string pcfldcsInput, string pcsldcsInput, string pctldcsInput)
        {
            decimal? pcfldcs = string.IsNullOrEmpty(pcfldcsInput) ? (decimal?)null : decimal.Parse(pcfldcsInput);
            decimal? pcsldcs = string.IsNullOrEmpty(pcsldcsInput) ? (decimal?)null : decimal.Parse(pcsldcsInput);
            decimal? pctldcs = string.IsNullOrEmpty(pctldcsInput) ? (decimal?)null : decimal.Parse(pctldcsInput);

            NewRule().LDCSConditionMet(pcfldcs, pcsldcs, pctldcs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            decimal? pcfldcs = 40.5m;
            decimal? pcsldcs = 32.5m;
            decimal? pctldcs = 32.5m;

            NewRule().ConditionMet(learnStartDate, pcfldcs, pcsldcs, pctldcs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnStartDate = new DateTime(2009, 01, 01);
            decimal? pcfldcs = 40.5m;
            decimal? pcsldcs = 32.5m;
            decimal? pctldcs = 32.5m;

            NewRule().ConditionMet(learnStartDate, pcfldcs, pcsldcs, pctldcs).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCFLDCSNullable = 10.0m,
                            PCSLDCSNullable = 10.0m,
                            PCTLDCSNullable = 50.5m
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
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCFLDCSNullable = 10.0m,
                            PCSLDCSNullable = 40.0m,
                            PCTLDCSNullable = 50.0m
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
        public void ValidateNoError_NullHEEntity()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = null
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
            decimal? pcfldcs = 40.5m;
            decimal? pcsldcs = 32.5m;
            decimal? pctldcs = 32.5m;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCFLDCS, pcfldcs)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCSLDCS, pcsldcs)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCTLDCS, pctldcs)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(pcfldcs, pcsldcs, pctldcs);

            validationErrorHandlerMock.Verify();
        }

        private PCFLDCS_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCFLDCS_01Rule(validationErrorHandler);
        }
    }
}
