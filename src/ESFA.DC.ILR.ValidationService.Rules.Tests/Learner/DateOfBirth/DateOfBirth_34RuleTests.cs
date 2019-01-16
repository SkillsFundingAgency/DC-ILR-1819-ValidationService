﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
    public class DateOfBirth_34RuleTests : AbstractRuleTests<DateOfBirth_34Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_34");
        }

        [Fact]
        public void ValidatePasses_OutsideAgeRange()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(18);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1999, 9, 1),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.HNS
                    }
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePassesIrrelevantFamType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    }
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePassesIrrelevantFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.HNS
                    }
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePassesEHCCodeFound()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.HNS
                    },
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.EHC
                    }
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.HNS
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var academicYearServiceMock = new Mock<IAcademicYearDataService>();
            academicYearServiceMock.Setup(m => m.AugustThirtyFirst()).Returns(new DateTime(2018, 8, 31));

            var dateTimeServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeServiceMock
                .Setup(m => m.AgeAtGivenDate(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(20);

            var testLearner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(1998, 8, 31),
                LearnerFAMs = new List<TestLearnerFAM>
                {
                    new TestLearnerFAM
                    {
                        LearnFAMType = LearnerFAMTypeConstants.HNS
                    }
                },
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, academicYearServiceMock.Object, dateTimeServiceMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private DateOfBirth_34Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new DateOfBirth_34Rule(academicYearDataService, validationErrorHandler, dateTimeQueryService);
        }

        private void VerifyErrorHandlerMock(ValidationErrorHandlerMock errorHandlerMock, int times = 0)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Exactly(times));
        }
    }
}
