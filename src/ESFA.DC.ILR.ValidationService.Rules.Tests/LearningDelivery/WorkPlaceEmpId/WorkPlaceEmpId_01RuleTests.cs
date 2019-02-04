using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_01RuleTests : AbstractRuleTests<WorkPlaceEmpId_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceEmpId_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(false);

            NewRule(employersDataService: employersDataServiceMock.Object).ConditionMet(123456).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_WorkPlaceEmpIdNull()
        {
            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(false);

            NewRule(employersDataService: employersDataServiceMock.Object).ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_WorkPlaceEmpId_999999999()
        {
            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(false);

            NewRule(employersDataService: employersDataServiceMock.Object).ConditionMet(999999999).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_WorkPlaceEmpId_LookUpExists()
        {
            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(true);

            NewRule(employersDataService: employersDataServiceMock.Object).ConditionMet(123456798).Should().BeFalse();
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
                                WorkPlaceEmpIdNullable = 987654321
                            },
                        }
                    }
                }
            };

            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(employersDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                                WorkPlaceEmpIdNullable = 999999999
                            },
                        }
                    }
                }
            };

            var employersDataServiceMock = new Mock<IEmployersDataService>();

            employersDataServiceMock.Setup(x => x.IsValid(It.IsAny<int>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(employersDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, 100000012)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(100000012);

            validationErrorHandlerMock.Verify();
        }

        private WorkPlaceEmpId_01Rule NewRule(
            IEmployersDataService employersDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceEmpId_01Rule(employersDataService, validationErrorHandler);
        }
    }
}
