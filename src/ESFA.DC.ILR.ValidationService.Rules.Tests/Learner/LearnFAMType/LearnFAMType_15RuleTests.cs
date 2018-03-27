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
    public class LearnFAMType_15RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF",
                    LearnFAMCodeNullable = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXX"
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, It.IsAny<string>(), It.IsAny<long>()))
                .Returns(true);
            learnerFamQueryServiceMock.Setup(x => x.HasAnyLearnerFAMTypes(famTypesList, It.IsAny<IEnumerable<string>>())).Returns(false);

            var rule = NewRule(null, learnerFamQueryServiceMock.Object);
            rule.ConditionMet(famTypesList).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF",
                    LearnFAMCodeNullable = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "SEN"
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, "MCF", 1))
                .Returns(true);

            learnerFamQueryServiceMock.Setup(x => x.HasAnyLearnerFAMTypes(famTypesList, It.IsAny<IEnumerable<string>>())).Returns(true);

            var rule = NewRule(null, learnerFamQueryServiceMock.Object);
            rule.ConditionMet(famTypesList).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetForValidFamType_True()
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "1"
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, It.IsAny<string>(), It.IsAny<long>()))
                .Returns(true);
            var rule = NewRule(null, learnerFamQueryServiceMock.Object);
            rule.ConditionMetForValidFamType(famTypesList).Should().BeTrue();
        }

        [Theory]
        [InlineData("MCF", 2)]
        [InlineData("XXX", 1)]
        public void ConditionMetForValidFamType_False(string famType, long famCode)
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = famType,
                    LearnFAMCodeNullable = famCode
                }
            };
            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasLearnerFAMCodeForType(famTypesList, It.IsAny<string>(), It.IsAny<long>()))
                .Returns(false);
            var rule = NewRule(null, learnerFamQueryServiceMock.Object);

            rule.ConditionMetForValidFamType(famTypesList).Should().BeFalse();
        }

        [Theory]
        [InlineData("SEN")]
        [InlineData("EHC")]
        public void ConditionMetSENOrEHCNotFound_False(string famType)
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = famType
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXXX"
                }
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasAnyLearnerFAMTypes(famTypesList, It.IsAny<IEnumerable<string>>()))
                .Returns(true);
            var rule = NewRule(null, learnerFamQueryServiceMock.Object);

            rule.ConditionMetSENOrEHCNotFound(famTypesList).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetSENOrEHCNotFound_True()
        {
            var famTypesList = new List<ILearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "AAAA"
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXXX"
                }
            };
            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock
                .Setup(x => x.HasAnyLearnerFAMTypes(famTypesList, It.IsAny<IEnumerable<string>>()))
                .Returns(false);
            var rule = NewRule(null, learnerFamQueryServiceMock.Object);

            rule.ConditionMetSENOrEHCNotFound(famTypesList).Should().BeTrue();
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
                        LearnFAMType = "MCF",
                        LearnFAMCodeNullable = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "AAA"
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_15", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "MCF", 1)).Returns(true);
            learnerFamQueryServiceMock.Setup(x => x.HasAnyLearnerFAMTypes(learner.LearnerFAMs, It.IsAny<IEnumerable<string>>())).Returns(false);

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
                        LearnFAMType = "MCF",
                        LearnFAMCodeNullable = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "SEN"
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_15", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(x => x.HasLearnerFAMCodeForType(learner.LearnerFAMs, "MCF", 1)).Returns(true);
            learnerFamQueryServiceMock.Setup(x => x.HasAnyLearnerFAMTypes(learner.LearnerFAMs, It.IsAny<IEnumerable<string>>())).Returns(true);

            var rule = NewRule(validationErrorHandlerMock.Object, learnerFamQueryServiceMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LearnFAMType_15Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerFAMQueryService learnerFamQueryService = null)
        {
            return new LearnFAMType_15Rule(validationErrorHandler, learnerFamQueryService);
        }
    }
}