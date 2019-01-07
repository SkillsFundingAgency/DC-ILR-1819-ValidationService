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
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R63RuleTests : AbstractRuleTests<R63Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R63");
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(TypeOfFunding.AdultSkills, TypeOfAim.CoreAim16To19ExcludingApprenticeships).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfAim.AimNotPartOfAProgramme).Should().BeTrue();
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
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships,
                        AimType = TypeOfAim.AimNotPartOfAProgramme
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
                        FundModel = TypeOfFunding.CommunityLearning,
                        AimType = TypeOfAim.CoreAim16To19ExcludingApprenticeships
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearner testLearner = null;

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        public R63Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R63Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
