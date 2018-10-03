using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_23RuleTests : AbstractRuleTests<DateOfBirth_23Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_23");
        }

        [Fact]
        public void FundModelCondtionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondtionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthCondtionMet_True()
        {
            NewRule().DateOfBirthConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthCondtionMet_False()
        {
            var dateOfBirth = new DateTime(2005, 01, 01);

            NewRule().DateOfBirthConditionMet(dateOfBirth).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_ADL()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_OLASS()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 10,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DOB()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(new DateTime(2000, 1, 1), learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMs()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 99,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = learningDeliveries
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = 99,
                    LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = learningDeliveries
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_23Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_23Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
