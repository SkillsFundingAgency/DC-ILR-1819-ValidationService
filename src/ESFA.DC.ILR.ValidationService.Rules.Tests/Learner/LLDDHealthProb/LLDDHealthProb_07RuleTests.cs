using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_07RuleTests : AbstractRuleTests<LLDDHealthProb_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDHealthProb_07");
        }

        [Fact]
        public void ValidatePasses_IrrelevantFundModelFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = TypeOfFunding.CommunityLearning
                    },
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.OtherAdult
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_AgeExceptionApplies()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(26);

            var derivedDataMock = new Mock<IDerivedData_06Rule>();
            derivedDataMock.Setup(m => m.Derive(It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(new DateTime(2018, 9, 1));

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1978, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = TypeOfFunding.CommunityLearning
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, derivedDataMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_PlannedHoursTooLow()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 5,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = TypeOfFunding.CommunityLearning
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_IrrelevantHealthProblem()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.NoLearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = TypeOfFunding.CommunityLearning
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_HealthProblemRecordFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = new List<TestLLDDAndHealthProblem>
                {
                    new TestLLDDAndHealthProblem()
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = TypeOfFunding.CommunityLearning
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.CommunityLearning)]
        [InlineData(TypeOfFunding.NotFundedByESFA)]
        public void ValidateFails(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock.Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(20);

            var derivedDataMock = new Mock<IDerivedData_06Rule>();
            derivedDataMock.Setup(m => m.Derive(It.IsAny<IEnumerable<ILearningDelivery>>()))
                .Returns(new DateTime(2018, 9, 1));

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.SOF_LA
                            }
                        }
                    }
                }
            };

            var testLearnerPasses = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 9, 1),
                PlanLearnHoursNullable = 11,
                LLDDHealthProb = LLDDHealthProblemConstants.LearningDifficulty,
                LLDDAndHealthProblems = null,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnStartDate = new DateTime(2018, 9, 1),
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.SOF_LA
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    }
                }
            };

            var rule = NewRule(validationErrorHandlerMock.Object, derivedDataMock.Object, dateTimeServiceMock.Object);
            rule.Validate(testLearner);
            rule.Validate(testLearnerPasses);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private LLDDHealthProb_07Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_06Rule dd06 = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LLDDHealthProb_07Rule(dd06, dateTimeQueryService, validationErrorHandler);
        }

        private void VerifyErrorHandlerMock(ValidationErrorHandlerMock errorHandlerMock, int times = 0)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Exactly(times));
        }
    }
}
