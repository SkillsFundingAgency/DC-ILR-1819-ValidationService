using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R91RuleTests : AbstractRuleTests<R91Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R91");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(36).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(70).Should().BeTrue();
        }

        [Fact]
        public void LearnAimRefConditionMet_False()
        {
            NewRule().LearnAimRefConditionMet("Z0002347").Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefConditionMet_True()
        {
            NewRule().LearnAimRefConditionMet("ZESF0001").Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            NewRule().CompStatusConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            NewRule().CompStatusConditionMet(2).Should().BeTrue();
        }

        [Theory]
        [InlineData(70, "Z000234", 2)]
        [InlineData(70, "ZESF0001", 6)]
        [InlineData(10, "ZWRKX002", 2)]
        [InlineData(10, "ZWRKX002", 3)]
        public void Validate_Error(int fundModel, string learnAimRef, int compStatus)
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 6,
                        ConRefNumber = "ESF-123445679"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        CompStatus = compStatus,
                        ConRefNumber = "ESF-999999999"
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
                        FundModel = 70,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2,
                        ConRefNumber = "ESF-999999999"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearnAimReferenceDifferent()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 1,
                        ConRefNumber = "ESF-6013353315"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NoFundModel()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2,
                        ConRefNumber = "ESF-999999999"
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        LearnAimRef = "ZESF0001",
                        CompStatus = 2,
                        ConRefNumber = "ESF-999999999"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FundModel", 70)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnAimRef", "ZESF0001")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("CompStatus", 2)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(70, "ZESF0001", 2);
            validationErrorHandlerMock.Verify();
        }

        public R91Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R91Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
