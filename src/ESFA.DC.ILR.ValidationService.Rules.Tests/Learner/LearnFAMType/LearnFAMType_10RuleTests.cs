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
    public class LearnFAMType_10RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();

            var famTypesList = SetupLearnerFams(5, "LSR");
            rule.ConditionMet(famTypesList).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule();

            var famTypesList = SetupLearnerFams(4, "LSR");
            rule.ConditionMet(famTypesList).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            var rule = NewRule();
            rule.ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Invalid()
        {
            var rule = NewRule();
            var famTypesList = SetupLearnerFams(5, "HCN");
            rule.ConditionMet(famTypesList).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = SetupLearnerFams(5, "LSR")
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_10", null, null, null);
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
                LearnerFAMs = SetupLearnerFams(4, "LSR")
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_10", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LearnFAMType_10Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_10Rule(validationErrorHandler);
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
    }
}