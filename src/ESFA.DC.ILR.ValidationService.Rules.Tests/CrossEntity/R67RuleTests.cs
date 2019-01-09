using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R67RuleTests : AbstractRuleTests<R67Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R67");
        }

        [Fact]
        public void Validate_Null_True()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R67, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        }

        [Fact]
        public void ConditionMet_Null_False()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 123,
                    WorkPlaceStartDate = new DateTime(2018, 10, 11)
                },

                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 123,
                    WorkPlaceStartDate = new DateTime(2018, 10, 11)
                },
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 124,
                    WorkPlaceStartDate = new DateTime(2018, 10, 11)
                },
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = null,
                    WorkPlaceStartDate = new DateTime(2018, 10, 11)
                }
            };
            NewRule().ConditionMet(learningDeliveryWorkPlacements).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "2018-10-10")]
        [InlineData(999, "2018-10-10")]
        [InlineData(123, "2018-11-11")]
        [InlineData(999, "2018-11-11")]
        public void ConditionMet_False(int? employerId, string startDate)
        {
            var learningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
            {
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = 123,
                    WorkPlaceStartDate = new DateTime(2018, 10, 10)
                },
                new TestLearningDeliveryWorkPlacement()
                {
                    WorkPlaceEmpIdNullable = employerId,
                    WorkPlaceStartDate = DateTime.Parse(startDate)
                }
            };
            NewRule().ConditionMet(learningDeliveryWorkPlacements).Should().BeFalse();
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearner = new TestLearner
            {
                LearnRefNumber = "learnRef1",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 150,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 9999,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R67, "learnRef1", 1, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
            validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R67, "learnRef1", 2, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearnRefNumber = "learnRef1",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = null,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2017, 10, 10)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 9999,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceEmpIdNullable = 123,
                                WorkPlaceStartDate = new DateTime(2018, 10, 10)
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R67, "learnRef1", It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceEmpId, 100)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 100);

            validationErrorHandlerMock.Verify();
        }

        private R67Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R67Rule(validationErrorHandler);
        }
    }
}
