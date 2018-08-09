using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R07RuleTests : AbstractRuleTests<R07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R07");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimSeqNumber = 1
                },
                new TestLearningDelivery
                {
                    AimSeqNumber = 2
                },
                new TestLearningDelivery
                {
                    AimSeqNumber = 2
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimSeqNumber = 1
                },
                new TestLearningDelivery
                {
                    AimSeqNumber = 2
                }
            };

            NewRule().ConditionMet(learningDeliveries).Should().BeFalse();
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
                        AimSeqNumber = 1
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                        AimSeqNumber = 1
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2
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
            var aimSeqNumbers = new List<int> { 1, 2 };

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimSeqNumber", "1, 2")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimSeqNumbers);

            validationErrorHandlerMock.Verify();
        }

        private R07Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R07Rule(validationErrorHandler);
        }
    }
}
