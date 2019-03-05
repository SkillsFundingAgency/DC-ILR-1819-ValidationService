using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceMode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceMode
{
    public class WorkPlaceMode_01RuleTests : AbstractRuleTests<WorkPlaceMode_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceMode_01");
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void ConditionMetMeetsExpectation(bool isValidWorkPlaceMode, bool expectation)
        {
            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock.Setup(x => x.IsValidWorkPlaceMode(It.IsAny<int>())).Returns(isValidWorkPlaceMode);

            NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object).ConditionMet(new TestLearningDeliveryWorkPlacement()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                      LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceMode = 0
                            },
                        }
                    }
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.IsValidWorkPlaceMode(It.IsAny<int>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryWorkPlacements = new List<TestLearningDeliveryWorkPlacement>()
                        {
                            new TestLearningDeliveryWorkPlacement
                            {
                                WorkPlaceMode = 0
                            },
                        }
                    }
                }
            };

            var learningDeliveryWorkPlacementQueryServiceMock = new Mock<ILearningDeliveryWorkPlacementQueryService>();
            learningDeliveryWorkPlacementQueryServiceMock
                .Setup(x => x.IsValidWorkPlaceMode(It.IsAny<int>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryWorkPlacementQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceMode, WorkPlaceModeConstants.ExternalWorkPlacement)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new TestLearningDeliveryWorkPlacement { WorkPlaceMode = 2 });

            validationErrorHandlerMock.Verify();
        }

        private WorkPlaceMode_01Rule NewRule(
            ILearningDeliveryWorkPlacementQueryService learningDeliveryWorkPlacementQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceMode_01Rule(learningDeliveryWorkPlacementQueryService, validationErrorHandler);
        }
    }
}
