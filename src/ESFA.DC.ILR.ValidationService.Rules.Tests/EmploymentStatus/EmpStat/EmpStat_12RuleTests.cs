using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_12RuleTests : AbstractRuleTests<EmpStat_12Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpStat_12");
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 25;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_False_Null()
        {
            int? progType = null;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void EmpStatConditionMet_True()
        {
            var empStats = new List<int> { 11 };
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void EmpStatConditionMet_False_EmpStat()
        {
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var empStats = new List<int> { 10 };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(10);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void EmpStatConditionMet_True_NoDateMatch()
        {
            var learnStartDate = new DateTime(2018, 7, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(0);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        /// <summary>
        /// If you supply multiple employment statuses, the latest should win over the first if the statuses are 11 -> 10
        /// </summary>
        [Fact]
        public void EmpStatConditionMet_False_MultipleEmpRecords()
        {
            var learnStartDate = new DateTime(2018, 7, 25);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 7, 24)
                },
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 7, 25)
                }
            };

            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();

            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(10);

            NewRule(learnerEmploymentStatusQueryService: learnerEmploymentStatusQueryServiceMock.Object).EmpStatConditionMet(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var empStats = new List<int> { 11 };
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var empStats = new List<int> { 11 };
            var aimType = 2;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            var empStats = new List<int> { 11 };
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_NoApplicableEmpStat()
        {
            var empStats = new List<int>();
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 9, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(0);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAMCodes()
        {
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(11);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var empStats = new List<int> { 11 };
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var empStats = new List<int> { 11 };
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 8, 1)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "353"
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[0]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_MultipleEmploymentStatus_NoError()
        {
            var empStats = new List<int> { 11, 10 };
            var aimType = 1;
            var progType = 24;
            var learnStartDate = new DateTime(2018, 7, 25);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 11,
                    DateEmpStatApp = new DateTime(2018, 7, 24)
                },
                new TestLearnerEmploymentStatus
                {
                    EmpStat = 10,
                    DateEmpStatApp = new DateTime(2018, 7, 25)
                }
            };

            var famCodes = new List<string> { "353", "354", "355" };
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner()
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dd07Mock = new Mock<IDD07>();
            var learnerEmploymentStatusQueryServiceMock = new Mock<ILearnerEmploymentStatusQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learnerEmploymentStatusQueryServiceMock.Setup(qs => qs.EmpStatForDateEmpStatApp(learnerEmploymentStatuses, learnStartDate)).Returns(empStats[1]);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", famCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerEmploymentStatusQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2018")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 8, 1));

            validationErrorHandlerMock.Verify();
        }

        private EmpStat_12Rule NewRule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService = null,
            IDD07 dd07 = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpStat_12Rule(learnerEmploymentStatusQueryService, dd07, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
