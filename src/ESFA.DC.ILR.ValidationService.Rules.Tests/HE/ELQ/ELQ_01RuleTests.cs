using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.ELQ;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.ELQ
{
    public class ELQ_01RuleTests : AbstractRuleTests<ELQ_01RuleTests>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ELQ_01");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                ELQNullable = 1
            };
            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False_NullEntity()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                NETFEENullable = null
            };
            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, LearningDeliveryFAMTypeConstants.SOF, "1")]
        public void ConditionMet_True(int fundModel, string learningDelFAMType, string learningDelFAMCode)
        {
            TestLearningDeliveryHE learningDeliveryHEEntity = new TestLearningDeliveryHE()
            {
                ELQNullable = null
            };
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), learningDelFAMType, learningDelFAMCode)).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(fundModel, learningDeliveryHEEntity, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(1, null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMType()
        {
            TestLearningDeliveryHE learningDeliveryHEEntity = new TestLearningDeliveryHE()
            {
                ELQNullable = null
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.ACT, "1")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(10, null, null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            ELQNullable = null
                        },
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                                LearnDelFAMCode = "1"
                            }
                        }
                    }
                },
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.SOF, "1")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            ELQNullable = 1
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 99)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(99, LearningDeliveryFAMTypeConstants.SOF, "1");

            validationErrorHandlerMock.Verify();
        }

        private ELQ_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ELQ_01Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
