using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_04RuleTests : AbstractRuleTests<PriorAttain_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_04");
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        public void PriorAttainConditionMet_True(int priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void PriorAttainConditionMet_False(int? priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(1).Should().BeFalse();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void ProgTypeConditionMet_True(int progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var priorAttain = 4;
            var fundModel = 35;
            var progType = 2;

            NewRule().ConditionMet(priorAttain, fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalsePriorAttain()
        {
            int? priorAttain = null;
            var fundModel = 35;
            var progType = 2;

            NewRule().ConditionMet(null, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var priorAttain = 4;
            var fundModel = 1;
            var progType = 2;

            NewRule().ConditionMet(priorAttain, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var priorAttain = 4;
            var fundModel = 35;
            int? progType = null;

            NewRule().ConditionMet(priorAttain, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                PriorAttainNullable = 4,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 2
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
                        FundModel = 35,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private PriorAttain_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_04Rule(validationErrorHandler);
        }
    }
}
