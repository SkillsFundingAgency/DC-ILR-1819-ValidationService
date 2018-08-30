using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnActEndDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnActEndDate
{
    public class LearnActEndDate_04RuleTests : AbstractRuleTests<LearnActEndDate_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnActEndDate_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2017, 1, 2)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Dates()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 2), new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(c => c.FilePreparationDate()).Returns(new DateTime(2017, 1, 1));

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2017, 1, 2),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(c => c.FilePreparationDate()).Returns(new DateTime(2017, 1, 1));

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private LearnActEndDate_04Rule NewRule(IFileDataService fileDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnActEndDate_04Rule(fileDataService, validationErrorHandler);
        }
    }
}
