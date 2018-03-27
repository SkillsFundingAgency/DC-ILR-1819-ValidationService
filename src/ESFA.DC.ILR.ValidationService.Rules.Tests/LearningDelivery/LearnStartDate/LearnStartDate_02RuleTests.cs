using System;
using System.Linq.Expressions;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_02RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2005, 1, 1), new DateTime(2017, 8, 1)).Should().BeTrue();
        }

        [Theory]
        [InlineData(2016, 1, 1)]
        [InlineData(2030, 1, 1)]
        public void ConditionMet_False(int year, int month, int day)
        {
            NewRule().ConditionMet(new DateTime(year, month, day), new DateTime(2017, 8, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2017, 8, 1)).Should().BeFalse();
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
                        LearnStartDateNullable = new DateTime(2015, 1, 1),
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();

            validationDataServiceMock.SetupGet(vd => vd.AcademicYearStart).Returns(new DateTime(2017, 8, 1));

            var rule = NewRule(validationDataServiceMock.Object);

            rule.Validate(learner);
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDateNullable = new DateTime(2005, 1, 1),
                    }
                }
            };

            var validationDataServiceMock = new Mock<IValidationDataService>();

            validationDataServiceMock.SetupGet(vd => vd.AcademicYearStart).Returns(new DateTime(2017, 8, 1));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnStartDate_02", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new LearnStartDate_02Rule(validationDataServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private LearnStartDate_02Rule NewRule(IValidationDataService validationDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnStartDate_02Rule(validationDataService, validationErrorHandler);
        }
    }
}
