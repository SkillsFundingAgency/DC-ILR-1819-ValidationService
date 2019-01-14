using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProvSpecDelMonOccur;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ProvSpecDelMonOccur
{
    public class ProvSpecDelMonOccur_01RuleTests : AbstractRuleTests<ProvSpecDelMonOccur_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProvSpecDelMonOccur_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            for (var letter = 'E'; letter <= 'Z'; letter++)
            {
                rule.ConditionMet(letter.ToString()).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        [InlineData("D")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ConditionMet_False(string provSpecDelMonOccur)
        {
            var rule = NewRule();
            rule.ConditionMet(provSpecDelMonOccur).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "E"
                            }
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
        public void Validate_False_MultipleLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "E"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "A"
                            }
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
        public void Validate_True_Null()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "A"
                            }
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
        public void Validate_True_MultipleLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "A"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "B"
                            }
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProvSpecDelMonOccur, "A")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");

            validationErrorHandlerMock.Verify();
        }

        private ProvSpecDelMonOccur_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProvSpecDelMonOccur_01Rule(validationErrorHandler);
        }
    }
}