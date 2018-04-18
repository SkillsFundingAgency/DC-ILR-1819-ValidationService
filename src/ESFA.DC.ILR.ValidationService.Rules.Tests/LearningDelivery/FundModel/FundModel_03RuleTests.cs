using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_03RuleTests : AbstractRuleTests<FundModel_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "ADL")).Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ConditionMet(10, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(99, new List<ILearningDeliveryFAM>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryFAMs()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "ADL")).Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object).ConditionMet(10, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "ADL")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                        FundModel = 99,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private FundModel_03Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_03Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
