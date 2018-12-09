using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R20RuleTests : AbstractRuleTests<R20Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R20");
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(null)]
        public void ApprenticeshipStandardsConditionMet_False(int? progType)
        {
            NewRule().ApprenticeshipStandardsConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_True()
        {
            NewRule().ApprenticeshipStandardsConditionMet(TypeOfLearningProgramme.HigherApprenticeshipLevel4).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            string learnAimRef = "ZESF98765";
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);

            NewRule(larsDataService: larsMock.Object).LARSConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string learnAimRef = "ZESF98765";
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsMock = new Mock<ILARSDataService>();

            larsMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(true);

            NewRule(larsDataService: larsMock.Object).LARSConditionMet(learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData("02/07/2015", "01/12/2015", 1)]
        [InlineData("01/12/2015", "02/07/2015", 2)]
        [InlineData("01/12/2015", null, 3)]
        public void LearnStartDateConditionMet_False(string learnStartDateString, string learnActEndDateString, int recordNo)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule().LearnStartDateConditionMet(learnStartDate, learnActEndDate, recordNo).Should().BeFalse();
        }

        [Theory]
        [InlineData("02/07/2015", null, 1)]
        [InlineData("02/07/2015", "01/12/2015", 2)]
        public void LearnStartDateConditionMet_True(string learnStartDateString, string learnActEndDateString, int recordNo)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            NewRule().LearnStartDateConditionMet(learnStartDate, learnActEndDate, recordNo).Should().BeTrue();
        }

        [Fact]
        public void GetLearningDeliveriesForCompetencyAim_Check()
        {
            var testLearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2018, 01, 01)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2018, 07, 02)
                    }
                };

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);

            NewRule(dd07: dd07Mock.Object).GetLearningDeliveriesForCompetencyAim(testLearningDeliveries).Should().BeEquivalentTo(testLearningDeliveries);
        }

        [Fact]
        public void GetLearningDeliveriesForCompetencyAim_NullCheck()
        {
            TestLearningDelivery[] testLearningDeliveries = null;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);

            NewRule(dd07: dd07Mock.Object).GetLearningDeliveriesForCompetencyAim(testLearningDeliveries).Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, "ZESF98765", "02/07/2015", "01/12/2015", 1)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, "ZESF98765", "02/07/2015", null, 2)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, "ZESF98765", "02/07/2015", "01/01/2015", 2)]
        public void ConditionMet_False(int? progType, string learnAimRef, string learnStartDateString, string learnActEndDateString, int recordNo)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).ConditionMet(progType, learnAimRef, learnStartDate, learnActEndDate, recordNo).Should().BeFalse();
        }

        [Theory]
        [InlineData("02/07/2015", null, 1)]
        [InlineData("02/07/2015", "01/12/2015", 2)]
        public void ConditionMet_True(string learnStartDateString, string learnActEndDateString, int recordNo)
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims("ZESF98765", frameWorkComponentTypes)).Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object)
                .ConditionMet(
                    TypeOfLearningProgramme.HigherApprenticeshipLevel4,
                    "ZESF98765",
                    learnStartDate,
                    learnActEndDate,
                    recordNo)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            string learnAimRef = "ZESF98765";
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2015, 07, 02),
                        LearnActEndDateNullable = null
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                        LearnStartDate = new DateTime(2015, 07, 02),
                        LearnActEndDateNullable = new DateTime(2015, 12, 01)
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);
            larsDataServiceMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    larsDataService: larsDataServiceMock.Object,
                    dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            HashSet<int?> frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };
            string learnAimRef = "ZESF98765";
            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        LearnStartDate = new DateTime(2015, 07, 02),
                        LearnActEndDateNullable = null
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(d => d.IsApprenticeship(2)).Returns(true);
            larsDataServiceMock.Setup(e => e.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, frameWorkComponentTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    larsDataService: larsDataServiceMock.Object,
                    dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ComponentAimInAProgramme)).Verifiable();
            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "02/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(e => e.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "25/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(
                TypeOfAim.ComponentAimInAProgramme,
                new DateTime(2018, 07, 02),
                new DateTime(2018, 10, 25));

            validationErrorHandlerMock.Verify();
        }

        public R20Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null,
            IDD07 dd07 = null)
        {
            return new R20Rule(
                validationErrorHandler: validationErrorHandler,
                lARSDataService: larsDataService,
                dd07: dd07);
        }
    }
}
