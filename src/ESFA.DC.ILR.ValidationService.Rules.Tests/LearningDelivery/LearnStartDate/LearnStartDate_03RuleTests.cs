using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_03RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2018, 8, 1), new DateTime(2018, 7, 31), 1, "N").Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2018, 7, 31), 1, "N").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2018, 7, 31), 1, "N").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet(new DateTime(2018, 8, 1), new DateTime(2018, 7, 31), 24, "N").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            NewRule().ConditionMet(new DateTime(2018, 8, 1), new DateTime(2018, 7, 31), 24, "Y").Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDateNullable = new DateTime(2015, 1, 1),
                ProgTypeNullable = 24
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var dd07Mock = new Mock<IDD07>();

            validationDataServiceMock.SetupGet(vd => vd.AcademicYearEnd).Returns(new DateTime(2017, 8, 1));
            dd07Mock.Setup(dd => dd.Derive(24)).Returns("Y");

            var rule = NewRule(dd07Mock.Object, validationDataServiceMock.Object);

            rule.Validate(learner);
        }

        [Fact]
        public void Validate_Errors()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDateNullable = new DateTime(2019, 1, 1),
                ProgTypeNullable = 1
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    learningDelivery,
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var dd07Mock = new Mock<IDD07>();

            validationDataServiceMock.SetupGet(vd => vd.AcademicYearEnd).Returns(new DateTime(2018, 7, 31));
            dd07Mock.Setup(dd => dd.Derive(1)).Returns("N");

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnStartDate_03", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new LearnStartDate_03Rule(dd07Mock.Object, validationDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private LearnStartDate_03Rule NewRule(IDD07 dd07 = null, IValidationDataService validationDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_03Rule(dd07, validationDataService, validationErrorHandler);
        }
    }
}
