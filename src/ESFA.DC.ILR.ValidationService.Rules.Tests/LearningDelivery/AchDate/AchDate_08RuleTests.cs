using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_08RuleTests : AbstractRuleTests<AchDate_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_08");
        }

        [Theory]
        [InlineData(24, 1)]
        [InlineData(25, 81)]
        public void ProgTypeConditionMet_True(int? progType, int fundModel)
        {
            NewRule().ProgTypeConditionMet(progType, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(20, 81)]
        [InlineData(25, 1)]
        [InlineData(null, 1)]
        public void ProgTypeConditionMet_False(int? progType, int fundModel)
        {
            NewRule().ProgTypeConditionMet(progType, fundModel).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(0).Should().BeFalse();
        }

        [Fact]
        public void OutcomeConditionMet_True()
        {
            NewRule().OutcomeConditionMet(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_True()
        {
            NewRule().AchDateConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void AchDateConditionMet_False()
        {
            NewRule().AchDateConditionMet(new DateTime(2015, 01, 01)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(25, 81, 1, 1, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet(0, 81, 1, 1, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            NewRule().ConditionMet(25, 81, 0, 1, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            NewRule().ConditionMet(25, 81, 1, 0, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AchDate()
        {
            NewRule().ConditionMet(25, 81, 1, 1, new DateTime(2015, 01, 01)).Should().BeFalse();
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
                        ProgTypeNullable = 25,
                        FundModel = 81,
                        AimType = 1,
                        OutcomeNullable = 1
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
                        ProgTypeNullable = 25,
                        FundModel = 81,
                        AimType = 1,
                        OutcomeNullable = 1,
                        AchDateNullable = new DateTime(2015, 01, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AchDate_08Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_08Rule(validationErrorHandler);
        }
    }
}
