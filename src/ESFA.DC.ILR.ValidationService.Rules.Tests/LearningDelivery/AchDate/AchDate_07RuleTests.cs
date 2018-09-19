using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_07RuleTests : AbstractRuleTests<AchDate_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_07");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2019, 1, 1), new DateTime(2018, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(new DateTime(2017, 1, 1), new DateTime(2020, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null, new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AchDateNullable = new DateTime(2019, 1, 1),
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fdc => fdc.FilePreparationDate()).Returns(new DateTime(2018, 1, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(new Mock<IFileDataService>().Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private AchDate_07Rule NewRule(IFileDataService fileDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_07Rule(fileDataService, validationErrorHandler);
        }
    }
}
