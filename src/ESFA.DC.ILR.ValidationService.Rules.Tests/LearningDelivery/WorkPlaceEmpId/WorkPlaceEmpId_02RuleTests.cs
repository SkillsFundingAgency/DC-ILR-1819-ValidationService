using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WorkPlaceEmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WorkPlaceEmpId
{
    public class WorkPlaceEmpId_02RuleTests : AbstractRuleTests<WorkPlaceEmpId_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("WorkPlaceEmpId_02");
        }

        [Theory]
        [InlineData(999999999, '1', false)]
        [InlineData(100000001, '1', false)]
        [InlineData(100000001, '2', true)]
        [InlineData(100000002, '2', false)]
        [InlineData(100000002, '3', true)]
        [InlineData(200000003, '3', false)]
        [InlineData(200000003, '4', true)]
        [InlineData(200000003, 'X', true)]
        [InlineData(20000000, 'X', true)]
        public void ConditionMetMeetsExpectation(int candidate, char checksum, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule14 = new Mock<IDerivedData_14Rule>(MockBehavior.Strict);
            ddRule14
                .SetupGet(x => x.InvalidLengthChecksum)
                .Returns('X');
            ddRule14
                .Setup(x => x.GetWorkPlaceEmpIdChecksum(candidate))
                .Returns(checksum);

            var sut = NewRule(ddRule14.Object, handler.Object);

            // act
            var result = sut.ConditionMet(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void ConditionMetReturnsFalseForNullWorkPlaceEmpId()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
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

            var derivedDataRule14Mock = new Mock<IDerivedData_14Rule>();
            derivedDataRule14Mock
                .Setup(x => x.GetWorkPlaceEmpIdChecksum(It.IsAny<int>()))
                .Returns('X');

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(derivedDataRule14Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                                WorkPlaceEmpIdNullable = 100000001
                            },
                        }
                    }
                }
            };

            var derivedDataRule14Mock = new Mock<IDerivedData_14Rule>();
            derivedDataRule14Mock
                .Setup(x => x.GetWorkPlaceEmpIdChecksum(It.IsAny<int>()))
                .Returns('1');

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(derivedDataRule14Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private WorkPlaceEmpId_02Rule NewRule(
            IDerivedData_14Rule derivedData14Rule = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new WorkPlaceEmpId_02Rule(derivedData14Rule, validationErrorHandler);
        }
    }
}
