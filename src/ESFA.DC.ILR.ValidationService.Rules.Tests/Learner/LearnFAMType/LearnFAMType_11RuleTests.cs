using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_11RuleTests
    {
        [Theory]
        [InlineData("NLM")]
        [InlineData("PPE")]
        [InlineData("EDF")]
        public void ConditionMet_True(string famType)
        {
            var rule = NewRule();

            var famTypesList = SetupLearnerFams(3, famType);

            rule.ConditionMet(famType, famTypesList).Should().BeTrue();
        }

        [Theory]
        [InlineData("NLM")]
        [InlineData("PPE")]
        [InlineData("EDF")]
        public void ConditionMet_False(string famType)
        {
            var rule = NewRule();

            var famTypesList = SetupLearnerFams(2, famType);

            rule.ConditionMet(famType, famTypesList).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ConditionMet_False_NullEmpty(string famType)
        {
            var rule = NewRule();
            rule.ConditionMet(famType, null).Should().BeFalse();
        }

        [Theory]
        [InlineData("NLM")]
        [InlineData("EDF")]
        [InlineData("PPE")]
        public void FamTypesListCheckConditionMet_True(string famType)
        {
            var rule = NewRule();
            rule.FamTypesListCheckConditionMet(famType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("XYZ")]
        public void FamTypesListCheckConditionMet_False(string famType)
        {
            var rule = NewRule();
            rule.FamTypesListCheckConditionMet(famType).Should().BeFalse();
        }

        [Fact]
        public void FamTypeCountConditionMet_True()
        {
            var rule = NewRule();
            var famTypesList = SetupLearnerFams(3, "PPE");
            rule.FamTypeCountConditionMet("PPE", famTypesList).Should().BeTrue();
        }

        [Fact]
        public void FamTypeCountConditionMet_False()
        {
            var rule = NewRule();
            var famTypesList = SetupLearnerFams(2, "PPE");
            rule.FamTypeCountConditionMet("ABC", famTypesList).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = SetupLearnerFams(3, "NLM")
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_11", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = SetupLearnerFams(2, "NLM")
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_11", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private List<ILearnerFAM> SetupLearnerFams(int count, string famType)
        {
            var items = new List<ILearnerFAM>();
            for (int counter = 0; counter < count; counter++)
            {
                items.Add(
                    new TestLearnerFAM()
                    {
                        LearnFAMType = famType
                    });
            }

            return items;
        }

        private LearnFAMType_11Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_11Rule(validationErrorHandler);
        }
    }
}