using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_02RuleTests : AbstractRuleTests<AchDate_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ay => ay.End()).Returns(new DateTime(2018, 9, 1));

            NewRule(academicYearDataServiceMock.Object).ConditionMet(new DateTime(2019, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ay => ay.End()).Returns(new DateTime(2018, 9, 1));

            NewRule(academicYearDataServiceMock.Object).ConditionMet(new DateTime(2017, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
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
                        AchDateNullable = new DateTime(2019, 1, 1)
                    }
                }
            };

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            academicYearDataServiceMock.Setup(ay => ay.End()).Returns(new DateTime(2018, 9, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private AchDate_02Rule NewRule(IAcademicYearDataService academicYearDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_02Rule(academicYearDataService, validationErrorHandler);
        }
    }
}
