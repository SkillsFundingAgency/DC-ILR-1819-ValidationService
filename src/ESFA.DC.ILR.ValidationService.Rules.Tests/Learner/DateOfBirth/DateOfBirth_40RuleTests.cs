using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_40RuleTests : AbstractRuleTests<DateOfBirth_40Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_40");
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePasses_OutsideAgeRange(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(18);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePassesIrrelevantFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.CommunityLearning,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesIrrelevantProgType(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.HigherApprenticeshipLevel4,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesIrrelevantAimType(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.AimNotPartOfAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesStartDateAfterCutOff(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2018, 7, 31),
                        LearnActEndDateNullable = new DateTime(2018, 9, 1)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesRestartException(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesNoCourseEndDate(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = null
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidatePassesCourse12Months(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2017, 7, 31)
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>()
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.OtherAdult)]
        public void ValidateFails(int fundModel)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1996, 7, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1)
                    },
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = null
                    },
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        AimType = TypeOfAim.ProgrammeAim,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2016, 7, 31),
                        LearnActEndDateNullable = new DateTime(2016, 9, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_40Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new DateOfBirth_40Rule(dateTimeQueryService, validationErrorHandler);
        }
    }
}
