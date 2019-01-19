using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R111RuleTests : AbstractRuleTests<R111Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R111");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(25).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var testlearningDeliveryFAMS = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDM"
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testlearningDeliveryFAMS, "LDM")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .LearningDeliveryFAMsConditionMet(testlearningDeliveryFAMS).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False_NullCheck()
        {
            TestLearningDeliveryFAM[] testlearningDeliveryFAMS = null;

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testlearningDeliveryFAMS, "LDM")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .LearningDeliveryFAMsConditionMet(testlearningDeliveryFAMS).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var testlearningDeliveryFAMS = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT"
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(testlearningDeliveryFAMS, "ACT")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .LearningDeliveryFAMsConditionMet(testlearningDeliveryFAMS).Should().BeTrue();
        }

        [Fact]
        public void LearnerEmploymentStatusConditionMet_False()
        {
            var testLearnerEmploymentStatus = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 15
                    }
                };

            NewRule().LearnerEmploymentStatusConditionMet(testLearnerEmploymentStatus).Should().BeFalse();
        }

        [Fact]
        public void LearnerEmploymentStatusConditionMet_False_NullCheck()
        {
            TestLearnerEmploymentStatus[] testLearnerEmploymentStatus = null;

            NewRule().LearnerEmploymentStatusConditionMet(testLearnerEmploymentStatus).Should().BeFalse();
        }

        [Theory]
        [InlineData(11)]
        [InlineData(12)]
        public void LearnerEmploymentStatusConditionMet_True(int empStat)
        {
            var testLearnerEmploymentStatus = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = empStat
                    }
                };

            NewRule().LearnerEmploymentStatusConditionMet(testLearnerEmploymentStatus).Should().BeTrue();
        }

        [Fact]
        public void LearnerEmploymentDuringLearningDeliveryConditionMet_False()
        {
            var testLearnerEmploymentStatus = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 15,
                        DateEmpStatApp = new DateTime(2018, 07, 01)
                    }
                };

            var testlearningDeliveryFAMS = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 09, 01),
                        LearnDelFAMDateToNullable = new DateTime(2018, 11, 01)
                    }
                };

            DateTime? empStartDateApp;
            int? empStat;
            DateTime? learnDelFAMDateFrom;
            DateTime? learnDelFAMDateTo;

            NewRule().LearnerEmploymentDuringLearningDeliveryConditionMet(
                testLearnerEmploymentStatus,
                testlearningDeliveryFAMS,
                out empStartDateApp,
                out empStat,
                out learnDelFAMDateFrom,
                out learnDelFAMDateTo).Should().BeFalse();

            empStartDateApp.Should().BeNull();
            empStat.Should().BeNull();
            learnDelFAMDateFrom.Should().BeNull();
            learnDelFAMDateTo.Should().BeNull();
        }

        [Fact]
        public void LearnerEmploymentDuringLearningDeliveryConditionMet_False_LearnerNullCheck()
        {
            TestLearnerEmploymentStatus[] testLearnerEmploymentStatus = null;

            var testlearningDeliveryFAMS = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 09, 01),
                        LearnDelFAMDateToNullable = new DateTime(2018, 11, 01)
                    }
                };

            DateTime? empStartDateApp;
            int? empStat;
            DateTime? learnDelFAMDateFrom;
            DateTime? learnDelFAMDateTo;

            NewRule().LearnerEmploymentDuringLearningDeliveryConditionMet(
                testLearnerEmploymentStatus,
                testlearningDeliveryFAMS,
                out empStartDateApp,
                out empStat,
                out learnDelFAMDateFrom,
                out learnDelFAMDateTo).Should().BeFalse();

            empStartDateApp.Should().BeNull();
            empStat.Should().BeNull();
            learnDelFAMDateFrom.Should().BeNull();
            learnDelFAMDateTo.Should().BeNull();
        }

        [Fact]
        public void LearnerEmploymentDuringLearningDeliveryConditionMet_False_DeliveryNullCheck()
        {
            var testLearnerEmploymentStatus = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 15,
                        DateEmpStatApp = new DateTime(2018, 07, 01)
                    }
                };

            TestLearningDeliveryFAM[] testlearningDeliveryFAMS = null;

            DateTime? empStartDateApp;
            int? empStat;
            DateTime? learnDelFAMDateFrom;
            DateTime? learnDelFAMDateTo;

            NewRule().LearnerEmploymentDuringLearningDeliveryConditionMet(
                testLearnerEmploymentStatus,
                testlearningDeliveryFAMS,
                out empStartDateApp,
                out empStat,
                out learnDelFAMDateFrom,
                out learnDelFAMDateTo).Should().BeFalse();

            empStartDateApp.Should().BeNull();
            empStat.Should().BeNull();
            learnDelFAMDateFrom.Should().BeNull();
            learnDelFAMDateTo.Should().BeNull();
        }

        [Theory]
        [InlineData("2018-11-01")]
        [InlineData(null)]
        public void LearnerEmploymentDuringLearningDeliveryConditionMet_True(string dateTo)
        {
            int empStatExpected = 15;
            DateTime dateEmpStatAppExpected = new DateTime(2018, 10, 01);
            DateTime? learnDelFAMDateFromExpected = new DateTime(2018, 09, 01);
            DateTime? learnDelFAMDateToExpected = string.IsNullOrEmpty(dateTo)
                ? (DateTime?)null : DateTime.Parse(dateTo);
            var testLearnerEmploymentStatus = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = empStatExpected,
                        DateEmpStatApp = dateEmpStatAppExpected
                    }
                };

            var testlearningDeliveryFAMS = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = learnDelFAMDateFromExpected,
                        LearnDelFAMDateToNullable = learnDelFAMDateToExpected
                    }
                };

            DateTime? empStartDateApp;
            int? empStat;
            DateTime? learnDelFAMDateFrom;
            DateTime? learnDelFAMDateTo;

            NewRule().LearnerEmploymentDuringLearningDeliveryConditionMet(
                testLearnerEmploymentStatus,
                testlearningDeliveryFAMS,
                out empStartDateApp,
                out empStat,
                out learnDelFAMDateFrom,
                out learnDelFAMDateTo).Should().BeTrue();

            empStartDateApp.Should().Be(dateEmpStatAppExpected);
            empStat.Should().Be(empStatExpected);
            learnDelFAMDateFrom.Should().Be(learnDelFAMDateFromExpected);
            learnDelFAMDateTo.Should().Be(learnDelFAMDateToExpected);
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 09, 01),
                        LearnDelFAMDateToNullable = new DateTime(2018, 11, 01)
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 09, 01),
                        LearnDelFAMDateToNullable = null
                    }
                };

            var testLearner = new TestLearner()
            {
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 15,
                        DateEmpStatApp = new DateTime(2018, 10, 01)
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 12,
                        DateEmpStatApp = new DateTime(2016, 01, 01)
                    }
                },
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ACT")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 09, 01),
                        LearnDelFAMDateToNullable = new DateTime(2018, 11, 01)
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "1",
                        LearnDelFAMDateFromNullable = new DateTime(2018, 11, 01),
                        LearnDelFAMDateToNullable = null
                    }
                };

            var testLearner = new TestLearner()
            {
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        EmpStat = 15,
                        DateEmpStatApp = new DateTime(2018, 10, 01)
                    }
                },
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 24,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "LDM")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            TestLearningDeliveryFAM[] learningDeliveryFAMs = null;
            TestLearner testLearner = null;

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "LDM")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryNullCheck()
        {
            TestLearningDeliveryFAM[] learningDeliveryFAMs = null;
            var testLearner = new TestLearner()
            {
                LearnerEmploymentStatuses = null,
                LearningDeliveries = null
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMType(learningDeliveryFAMs, "LDM")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            DateTime dateTimeEmpStatApp = new DateTime(2018, 11, 01);
            DateTime learnDelFAMTypeDateFrom = new DateTime(2018, 07, 01);
            DateTime learnDelFAMTypeDateTo = new DateTime(2018, 05, 01);

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("EmpStat", 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("DateEmpStatApp", "01/11/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("FundModel", 36)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMType", "ACT")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMCode", "034")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/05/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(2, dateTimeEmpStatApp, 36, "ACT", "034", learnDelFAMTypeDateFrom, learnDelFAMTypeDateTo);
            validationErrorHandlerMock.Verify();
        }

        public R111Rule NewRule (
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new R111Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
