using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_05RuleTests : AbstractRuleTests<DateOfBirth_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_05");
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(99, 3)]
        public void ConditionMet_True(int fundModel, int age)
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(age);

            var rule = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, learnStartDate, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(new DateTime(1988, 12, 25), new DateTime(2017, 8, 1), 1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel_DateOfBirth_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2017, 8, 1), 10).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Age()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(4);

            var rule = NewRule(dateTimeQueryServiceMock.Object);

            rule.ConditionMet(dateOfBirth, learnStartDate, 10).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var dateOfBirth = new DateTime(1988, 12, 25);
            var learnStartDate = new DateTime(2017, 8, 1);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(10);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2015")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2018")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 01, 01), new DateTime(2015, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        public DateOfBirth_05Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_05Rule(dateTimeQueryService, validationErrorHandler);
        }
    }
}
