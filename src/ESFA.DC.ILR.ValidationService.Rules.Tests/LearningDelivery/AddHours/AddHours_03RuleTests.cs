using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AddHours
{
    public class AddHours_03RuleTests : AbstractRuleTests<AddHours_03Rule>
    {
        private readonly IEnumerable<int> _basicSkillsType = new HashSet<int>() { 36, 37, 38, 39, 40, 41, 42 };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AddHours_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "AimRef";
            var learnStartDate = new DateTime(2018, 8, 1);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate)).Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnAimRef = "AimRef";
            var learnStartDate = new DateTime(2018, 8, 1);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate)).Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learnAimRef = "AimRef";
            var learnStartDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        AddHoursNullable = 1,
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void Validate_NoErrors(int? addHours)
        {
            var learnAimRef = "AimRef";
            var learnStartDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearnStartDate = learnStartDate,
                        AddHoursNullable = addHours,
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "AimRef")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AddHours", 1)).Verifiable();

            NewRule(null, validationErrorHandlerMock.Object).BuildErrorMessageParameters("AimRef", 1);

            validationErrorHandlerMock.Verify();
        }

        private AddHours_03Rule NewRule(ILARSDataService larsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AddHours_03Rule(larsDataService, validationErrorHandler);
        }
    }
}
