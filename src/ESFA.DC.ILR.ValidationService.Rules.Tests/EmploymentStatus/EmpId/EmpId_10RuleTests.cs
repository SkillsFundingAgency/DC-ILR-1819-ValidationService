using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_10RuleTests : AbstractRuleTests<EmpId_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpId_10");
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 2;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(50)]
        [InlineData(null)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var aimType = 1;

            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void AimTypeConditionMet_False(int aimType)
        {
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void EmpIdNotExistsOnLearnStartDate_True_DateMatch()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            NewRule().EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void EmpIdNotExistsOnLearnStartDate_True_DateLess()
        {
            var learnStartDate = new DateTime(2018, 8, 10);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            NewRule().EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Fact]
        public void EmpIdNotExistsOnLearnStartDate_False_DateAfter()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 2),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            NewRule().EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void EmpIdNotExistsOnLearnStartDate_False_EmpIdNotNull()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            NewRule().EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void EmpIdNotExistsOnLearnStartDate_False_NoEmploymentStatus()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>();

            NewRule().EmpIdNotExistsOnLearnStartDate(learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Fact]
        public void EmploymentStatusesInPaidEmployment_ReturnsEmptyCollection_NoEmploymentStatus()
        {
            NewRule().EmploymentStatusesInPaidEmployment(null).Should().BeEquivalentTo(new List<TestLearnerEmploymentStatus>());
        }

        [Theory]
        [InlineData(20)]
        [InlineData(1)]
        [InlineData(2)]
        public void EmploymentStatusesInPaidEmployment_ReturnsEmptyCollection(int empStat)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    EmpStat = empStat,
                }
            };

            NewRule().EmploymentStatusesInPaidEmployment(learnerEmploymentStatuses).Should().BeEquivalentTo(new List<TestLearnerEmploymentStatus>());
        }

        [Fact]
        public void EmploymentStatusesInPaidEmployment_ReturnsCollection()
        {
            var statusOne = new TestLearnerEmploymentStatus
            {
                EmpStat = 10,
            };

            var statusTwo = new TestLearnerEmploymentStatus
            {
                EmpStat = 11,
            };

            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                statusOne,
                statusTwo
            };

            NewRule().EmploymentStatusesInPaidEmployment(learnerEmploymentStatuses).Should().BeEquivalentTo(new List<TestLearnerEmploymentStatus> { statusOne });
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 2;
            var aimType = 1;
            var learnStartDate = new DateTime(2018, 8, 1);
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, false, 1, "2018-8-1", "2018-8-1", "2018-9-1")]
        [InlineData(2, true, 2, "2018-8-1", "2018-8-1", "2018-9-1")]
        [InlineData(2, true, 1, "2018-8-1", "2018-9-1", "2018-9-1")]
        [InlineData(2, true, 1, "2018-8-1", "2018-8-1", "2018-8-1")]
        public void ConditionMet_False(int? progType, bool mock, int aimType, DateTime learnStartDate, DateTime empstatAppOne, DateTime empstatAppTwo)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = empstatAppOne,
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = empstatAppTwo,
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mock);

            NewRule(dd07Mock.Object).ConditionMet(progType, aimType, learnStartDate, learnerEmploymentStatuses).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, false, 1, "2018-8-1", "2018-8-1", "2018-9-1", 10, 10)]
        [InlineData(2, true, 2, "2018-8-1", "2018-8-1", "2018-9-1", 10, 10)]
        [InlineData(2, true, 1, "2018-8-1", "2018-9-1", "2018-9-1", 10, 10)]
        [InlineData(2, true, 1, "2018-8-1", "2018-8-1", "2018-8-1", 10, 10)]
        [InlineData(2, true, 1, "2018-8-1", "2018-8-1", "2018-9-1", 11, 10)]
        [InlineData(2, true, 1, "2018-8-1", "2018-9-1", "2018-9-1", 10, 11)]
        [InlineData(2, true, 1, "2018-8-1", "2018-8-1", "2018-8-1", 11, 11)]
        public void Validate_NoError(int? progType, bool mock, int aimType, DateTime learnStartDate, DateTime empstatAppOne, DateTime empstatAppTwo, int empstatOne, int empstatTwo)
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = empstatAppOne,
                    EmpStat = empstatOne
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = empstatAppTwo,
                    EmpStat = empstatTwo,
                    EmpIdNullable = 11111
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = progType,
                        AimType = aimType,
                        LearnStartDate = learnStartDate
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mock);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(2)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoEmploymentStatuses()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1)
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(2)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoEmploymentStatusesInPaidEmployment()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 11
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1)
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(2)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1)
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(2)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveries()
        {
            var learnerEmploymentStatuses = new List<TestLearnerEmploymentStatus>
            {
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 8, 1),
                    EmpStat = 10
                },
                new TestLearnerEmploymentStatus
                {
                    DateEmpStatApp = new DateTime(2018, 9, 1),
                    EmpStat = 10,
                    EmpIdNullable = 11111
                }
            };

            var learner = new TestLearner
            {
                LearnerEmploymentStatuses = learnerEmploymentStatuses,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1)
                    },
                    new TestLearningDelivery
                    {
                        ProgTypeNullable = 2,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    },
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dm => dm.IsApprenticeship(2)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpStat", 10)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EmpId", string.Empty)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(10);

            validationErrorHandlerMock.Verify();
        }

        private EmpId_10Rule NewRule(IDerivedData_07Rule dd07 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpId_10Rule(dd07, validationErrorHandler);
        }
    }
}
