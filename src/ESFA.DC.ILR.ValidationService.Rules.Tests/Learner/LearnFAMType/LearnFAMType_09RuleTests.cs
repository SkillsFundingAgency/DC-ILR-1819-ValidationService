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
    public class LearnFAMType_09RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();

            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "HNS"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "HNS"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XYZ"
                }
            };

            rule.ConditionMet("HNS", famTypesList).Should().BeTrue();
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
        [InlineData("HNS")]
        [InlineData("EHC")]
        [InlineData("DLA")]
        [InlineData("SEN")]
        [InlineData("MCF")]
        [InlineData("ECF")]
        [InlineData("FME")]
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
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "ABC"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "ABC"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XYZ"
                }
            };
            rule.FamTypeCountConditionMet("ABC", famTypesList).Should().BeTrue();
        }

        [Fact]
        public void FamTypeCountConditionMet_False()
        {
            var rule = NewRule();
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "ABC"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXX"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XYZ"
                }
            };
            rule.FamTypeCountConditionMet("ABC", famTypesList).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "SEN",
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "SEN",
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "ABC",
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_09", null, null, null);
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
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "SEN",
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XXX",
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "ABC",
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_09", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LearnFAMType_09Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_09Rule(validationErrorHandler);
        }
    }
}