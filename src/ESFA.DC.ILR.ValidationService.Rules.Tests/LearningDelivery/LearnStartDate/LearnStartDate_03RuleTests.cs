using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_03RuleTests : AbstractRuleTests<LearnStartDate_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnStartDate_03");
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_FalseProgType()
        {
            int? progType = 24;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_FalseDD07()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 10, 01);
            var academicYearEnd = new DateTime(2019, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            NewRule(academicYearDataService: academicYearDataServiceMock.Object)
                .LearnStartDateConditionMet(learnStartDate)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learnStartDate = new DateTime(2018, 10, 01);
            var academicYearEnd = new DateTime(2019, 07, 31);

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            NewRule(academicYearDataService: academicYearDataServiceMock.Object)
                .LearnStartDateConditionMet(learnStartDate)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 10, 01);
            var academicYearEnd = new DateTime(2019, 07, 31);
            var progType = 1;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object, academicYearDataServiceMock.Object).ConditionMet(learnStartDate, progType)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseDD07()
        {
            var learnStartDate = new DateTime(2019, 10, 01);
            var academicYearEnd = new DateTime(2019, 07, 31);
            var progType = 1;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object, academicYearDataServiceMock.Object).ConditionMet(learnStartDate, progType)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLearnStartDate()
        {
            var learnStartDate = new DateTime(2018, 10, 01);
            var academicYearEnd = new DateTime(2019, 07, 31);
            var progType = 1;

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object, academicYearDataServiceMock.Object).ConditionMet(learnStartDate, progType)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var progType = 20;
            var academicYearEnd = new DateTime(2019, 07, 31);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2019, 10, 01),
                        ProgTypeNullable = progType
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var progType = 20;
            var academicYearEnd = new DateTime(2019, 07, 31);

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 10, 01),
                        ProgTypeNullable = progType
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.End()).Returns(academicYearEnd);

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/10/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 10, 01));

            validationErrorHandlerMock.Verify();
        }

        private LearnStartDate_03Rule NewRule(
            IDD07 dd07 = null,
            IAcademicYearDataService academicYearDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_03Rule(dd07, academicYearDataService, validationErrorHandler);
        }
    }
}
