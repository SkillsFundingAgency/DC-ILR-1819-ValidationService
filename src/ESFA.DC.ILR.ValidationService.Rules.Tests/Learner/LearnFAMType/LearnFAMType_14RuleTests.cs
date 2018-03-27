using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_14RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "SEN",
                    LearnFAMCodeNullable = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "EHC",
                    LearnFAMCodeNullable = 1
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, It.IsAny<string>(), It.IsAny<long>()))
                .Returns(true);
            var rule = NewRule(null, learnerFamQueryServiceMock.Object);
            rule.ConditionMet(famTypesList).Should().BeTrue();
        }

        [Theory]
        [InlineData("SEN")]
        [InlineData("EHC")]
        public void ConditionMet_False_SecondTypeNotFound(string famType)
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = famType,
                    LearnFAMCodeNullable = 1
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, famType, It.IsAny<long>()))
                .Returns(true);

            var rule = NewRule(null, learnerFamQueryServiceMock.Object);
            rule.ConditionMet(famTypesList).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 1)]
        [InlineData(null, 2)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(1, null)]
        public void ConditionMet_False_CodeNotMatched(long? code1, long? code2)
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "SEN",
                    LearnFAMCodeNullable = code1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "EHC",
                    LearnFAMCodeNullable = code2
                }
            };
            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(famTypesList, "SEN", It.IsAny<long>())).Returns(code1 == 1 ? true : false);
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(famTypesList, "EHC", It.IsAny<long>())).Returns(code2 == 1 ? true : false);

            var rule = NewRule(null, learnerFamQueryServiceMock.Object);

            rule.ConditionMet(famTypesList).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullEmpty()
        {
            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(null, It.IsAny<string>(), It.IsAny<long>()))
                .Returns(false);

            var rule = NewRule(null, learnerFamQueryServiceMock.Object);

            rule.ConditionMet(null).Should().BeFalse();
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
                        LearnFAMCodeNullable = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCodeNullable = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "ABC",
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_14", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "EHC", 1)).Returns(true);
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "SEN", 1)).Returns(true);

            var rule = NewRule(validationErrorHandlerMock.Object, learnerFamQueryServiceMock.Object);
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
                        LearnFAMCodeNullable = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "EHC",
                        LearnFAMCodeNullable = 2
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "ABC",
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_14", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "SEN", 1)).Returns(true);
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "EHC", 2)).Returns(true);

            var rule = NewRule(validationErrorHandlerMock.Object, learnerFamQueryServiceMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LearnFAMType_14Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerFAMQueryService learnerFamQueryService = null)
        {
            return new LearnFAMType_14Rule(validationErrorHandler, learnerFamQueryService);
        }
    }
}