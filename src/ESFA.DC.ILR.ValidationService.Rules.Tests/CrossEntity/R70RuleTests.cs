using System.Collections.Generic;
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
    public class R70RuleTests : AbstractRuleTests<R70Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R70");
        }

        [Fact]
        public void LearningDeliveryStandardCompAimConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery();

            NewRule().LearningDeliveryStandardCompAimConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryStandardCompAimConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery();

            NewRule().LearningDeliveryStandardCompAimConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryStandardProgAimConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 25,
                AimType = 3,
                StdCodeNullable = 1
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 23,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 24,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 1
                }
            };

            NewRule().LearningDeliveryStandardProgAimConditionMet(learningDelivery, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryStandardProgAimConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 25,
                AimType = 3,
                StdCodeNullable = 1
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 23,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 24,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 2
                }
            };

            NewRule().LearningDeliveryStandardProgAimConditionMet(learningDelivery, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_CompAim()
        {
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 25,
                AimType = 3,
                StdCodeNullable = 1
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 23,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 24,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 1
                }
            };

            NewRule().ConditionMet(null, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgAim()
        {
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 25,
                AimType = 3,
                StdCodeNullable = 1
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 23,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 24,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 1
                }
            };

            NewRule().ConditionMet(learningDelivery, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery
            {
                ProgTypeNullable = 25,
                AimType = 3,
                StdCodeNullable = 1
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    ProgTypeNullable = 23,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 24,
                    AimType = 3,
                    StdCodeNullable = 1
                },
                new TestLearningDelivery
                {
                    ProgTypeNullable = 25,
                    AimType = 1,
                    StdCodeNullable = 2
                }
            };

            NewRule().ConditionMet(learningDelivery, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 24,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        AimType = 1,
                        StdCodeNullable = 2
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_CompAim()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 24,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 24,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 23,
                        AimType = 1,
                        StdCodeNullable = 2
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ProgAim()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        FundModel = 35,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 24,
                        FundModel = 35,
                        AimType = 3,
                        StdCodeNullable = 1
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 25,
                        FundModel = 35,
                        AimType = 1,
                        StdCodeNullable = 1
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.AimType, 1));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 2));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 3));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.StdCode, 4));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1, 3, 4);
            validationErrorHandlerMock.Verify();
        }

        public R70Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R70Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
