using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_65RuleTests : AbstractRuleTests<LearnDelFAMType_65Rule>
    {
        //[Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_65");
        }

        //[Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var larsService = new Mock<ILARSDataService>();
            larsService
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(
                        It.IsAny<IEnumerable<int>>(),
                        It.IsAny<string>(),
                        It.IsAny<DateTime>()))
                .Returns(false);
            larsService
                .Setup(m => m.GetNotionalNVQLevelv2ForLearnAimRef(It.IsAny<string>()))
                .Returns("1");

            var dd07Mock = new Mock<IDD07>();
            dd07Mock
                .Setup(m => m.IsApprenticeship(It.IsAny<int?>()))
                .Returns(false);

            var dd28Mock = new Mock<IDerivedData_28Rule>();
            dd28Mock
                .Setup(m => m.IsAdultFundedUnemployedWithBenefits(It.IsAny<ILearner>()))
                .Returns(false);

            var dd29Mock = new Mock<IDerivedData_29Rule>();
            dd29Mock
                .Setup(m => m.IsInflexibleElementOfTrainingAim(It.IsAny<ILearner>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1997, 8, 1),
                PriorAttainNullable = 2,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        LearnAimRef = "00103212",
                        LearnStartDate = new DateTime(2017, 8, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                                LearnDelFAMCode = "2"
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsService.Object, dd07Mock.Object, dd28Mock.Object, dd29Mock.Object)
                .Validate(testLearner);
        }

        //[Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        //[Fact]
        public void ValidationPasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 25
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        //[Fact]
        public void ValidationPasses_IrrelevantFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 4,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ASL
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        //[Theory]
        //[InlineData(25)]
        public void ValidationFails(int fundingModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundingModel,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ASL
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<string>()));
        }

        private LearnDelFAMType_65Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null,
            IDD07 dd07 = null,
            IDerivedData_28Rule derivedData28Rule = null,
            IDerivedData_29Rule derivedData29Rule = null)
        {
            return new LearnDelFAMType_65Rule(validationErrorHandler, larsDataService, dd07, derivedData28Rule, derivedData29Rule);
        }
    }
}
