using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_54RuleTests : AbstractRuleTests<DateOfBirth_54Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_54");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var dd18Date = new DateTime(2017, 05, 01);
            var lastFridayInJuneForAcademicYear = new DateTime(2017, 06, 30);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(derivedData07: dd07Mock.Object).ConditionMet(progType, dd18Date, lastFridayInJuneForAcademicYear)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void ConditionMet_False_ProgType(int? progType)
        {
            var dd18Date = new DateTime(2017, 05, 01);
            var lastFridayInJuneForAcademicYear = new DateTime(2017, 06, 30);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(derivedData07: dd07Mock.Object).ConditionMet(progType, dd18Date, lastFridayInJuneForAcademicYear)
                .Should().BeFalse();
        }

        [Theory]
        [InlineData("2016/05/01")]
        [InlineData("2017/07/01")]
        [InlineData(null)]
        public void ConditionMet_False_DD18(string dd18)
        {
            DateTime? dd18Date = string.IsNullOrEmpty(dd18) ? (DateTime?)null : DateTime.Parse(dd18);

            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var lastFridayInJuneForAcademicYear = new DateTime(2017, 06, 30);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(derivedData07: dd07Mock.Object).ConditionMet(progType, dd18Date, lastFridayInJuneForAcademicYear)
                .Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var dd18Date = new DateTime(2017, 05, 01);
            var lastFridayInJuneForAcademicYear = new DateTime(2017, 06, 30);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(derivedData07: dd07Mock.Object).ConditionMet(progType, dd18Date, lastFridayInJuneForAcademicYear)
                .Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;

            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(2001, 05, 01),
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType
                    }
                }
            };

            var academicYearQueryService = new Mock<IAcademicYearQueryService>();
            academicYearQueryService.Setup(qs => qs.LastFridayInJuneForDateInAcademicYear(It.IsAny<DateTime>()))
                .Returns(new DateTime(2017, 06, 30));

            var dd18Mock = new Mock<IDerivedData_18Rule>();
            dd18Mock.Setup(dm => dm.GetApprenticeshipStandardProgrammeStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(new DateTime(2017, 05, 01));

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd18Mock.Object,
                    dd07Mock.Object,
                    academicYearQueryService.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;

            var learner = new TestLearner
            {
                DateOfBirthNullable = new DateTime(2001, 05, 01),
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType
                    }
                }
            };

            var academicYearQueryService = new Mock<IAcademicYearQueryService>();
            academicYearQueryService.Setup(qs => qs.LastFridayInJuneForDateInAcademicYear(It.IsAny<DateTime>()))
                .Returns(new DateTime(2017, 06, 30));

            var dd18Mock = new Mock<IDerivedData_18Rule>();
            dd18Mock.Setup(dm => dm.GetApprenticeshipStandardProgrammeStartDateFor(It.IsAny<ILearningDelivery>(), It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(new DateTime(2017, 05, 01));

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd18Mock.Object,
                    dd07Mock.Object,
                    academicYearQueryService.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private DateOfBirth_54Rule NewRule(
            IDerivedData_18Rule derivedData18 = null,
            IDerivedData_07Rule derivedData07 = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_54Rule(derivedData18, derivedData07, academicYearQueryService, validationErrorHandler);
        }
    }
}
