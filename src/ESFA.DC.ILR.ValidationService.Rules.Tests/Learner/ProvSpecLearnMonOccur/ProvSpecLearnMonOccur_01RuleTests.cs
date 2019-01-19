using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ProvSpecLearnMonOccur;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ProvSpecLearnMonOccur
{
    public class ProvSpecLearnMonOccur_01RuleTests : AbstractRuleTests<ProvSpecLearnMonOccur_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProvSpecLearnMonOccur_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            for (var letter = 'C'; letter <= 'Z'; letter++)
            {
                rule.ConditionMet(letter.ToString()).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ConditionMet_False(string provSpecLearnMonOccur)
        {
            var rule = NewRule();
            rule.ConditionMet(provSpecLearnMonOccur).Should().BeFalse();
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "X"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "A"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_False_Null()
        {
            var learner = new TestLearner()
            {
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = null
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProvSpecLearnMonOccur, "A")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");

            validationErrorHandlerMock.Verify();
        }

        private ProvSpecLearnMonOccur_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProvSpecLearnMonOccur_01Rule(validationErrorHandler);
        }
    }
}