using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ALSCost;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ALSCost
{
    public class ALSCost_02RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var learnerFamQueryService = new LearnerFAMQueryService();

            var learnerFams = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "XYZ"
                }
            };

            var rule = NewRule(learnerFamQueryService);

            rule.ConditionMet(10, learnerFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "XYZ")]
        [InlineData(100, "HNS")]
        public void ConditionMet_False(long? alsCost, string famType)
        {
            var learnerFamQueryService = new LearnerFAMQueryService();

            var fams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = famType
                }
            };

            var rule = NewRule(learnerFamQueryService);

            rule.ConditionMet(alsCost, fams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = SetupLearner(null);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ALSCost_02", null, null, null);

            var rule = NewRule(new LearnerFAMQueryService(), validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = SetupLearner("HNS");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ALSCost_02", null, null, null);

            var rule = NewRule(new LearnerFAMQueryService(), validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private ILearner SetupLearner(string learnFamType)
        {
            return new TestLearner()
            {
                ALSCostNullable = 10,
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = learnFamType
                    }
                }
            };
        }

        private ALSCost_02Rule NewRule(ILearnerFAMQueryService learnerFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ALSCost_02Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
