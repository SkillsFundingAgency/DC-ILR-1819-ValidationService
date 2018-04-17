using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.EmpOutcome.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_02RuleTests : AbstractRuleTests<EmpOutcome_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpOutcome_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var empOutcome = 1;

            var empOutcomeDataServiceMock = new Mock<IEmpOutcomeDataService>();

            empOutcomeDataServiceMock.Setup(ds => ds.Exists(empOutcome)).Returns(false);

            NewRule(empOutcomeDataServiceMock.Object).ConditionMet(empOutcome).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_EmpOutcome()
        {
            var empOutcome = 1;

            var empOutcomeDataServiceMock = new Mock<IEmpOutcomeDataService>();

            empOutcomeDataServiceMock.Setup(ds => ds.Exists(empOutcome)).Returns(true);

            NewRule(empOutcomeDataServiceMock.Object).ConditionMet(empOutcome).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var empOutcome = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EmpOutcomeNullable = empOutcome,
                    }
                }
            };

            var empOutcomeDataServiceMock = new Mock<IEmpOutcomeDataService>();

            empOutcomeDataServiceMock.Setup(ds => ds.Exists(empOutcome)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(empOutcomeDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EmpOutcomeNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpOutcome", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private EmpOutcome_02Rule NewRule(IEmpOutcomeDataService empOutcomeDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpOutcome_02Rule(empOutcomeDataService, validationErrorHandler);
        }
    }
}
