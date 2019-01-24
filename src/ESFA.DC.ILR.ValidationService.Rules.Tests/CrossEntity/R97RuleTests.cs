using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R97RuleTests : AbstractRuleTests<R97Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R97");
        }

        [Fact]
        public void LearnerEmploymentStatusConditionMet_False()
        {
            var testLearnerEmloymentStatus = new TestLearnerEmploymentStatus()
            {
                AgreeId = "123A01",
                EmpIdNullable = 11768,
                EmpStat = 10
            };

            var testPreviousLearnerEmloymentStatus = new TestLearnerEmploymentStatus()
            {
                AgreeId = "123A02",
                EmpIdNullable = 11769,
                EmpStat = 11
            };

            NewRule().LearnerEmploymentStatusConditionMet(
                testLearnerEmloymentStatus,
                testPreviousLearnerEmloymentStatus).Should().BeFalse();
        }

        [Fact]
        public void LearnerEmploymentStatusConditionMet_True()
        {
            var testLearnerEmloymentStatus = new TestLearnerEmploymentStatus()
            {
                AgreeId = "123A01",
                EmpIdNullable = 11768,
                EmpStat = 10
            };

            var testPreviousLearnerEmloymentStatus = new TestLearnerEmploymentStatus()
            {
                AgreeId = "123a01",
                EmpIdNullable = 11768,
                EmpStat = 10
            };

            NewRule().LearnerEmploymentStatusConditionMet(
                testLearnerEmloymentStatus,
                testPreviousLearnerEmloymentStatus).Should().BeTrue();
        }

        [Fact]
        public void LearnerEmploymentStatusMonitoringConditionMet_False()
        {
            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "LOE",
                        ESMCode = 2
                    },
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "EII",
                        ESMCode = 3
                    }
                };

            var previousEmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "ISB",
                        ESMCode = 6
                    },
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "LHR",
                        ESMCode = 8
                    }
                };

            NewRule().LearnerEmploymentStatusMonitoringConditionMet(
                previousEmploymentStatusMonitorings,
                employmentStatusMonitorings).Should().BeFalse();
        }

        [Fact]
        public void LearnerEmploymentStatusMonitoringConditionMet_False_DataNullCheck()
        {
            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "LOE",
                        ESMCode = 2
                    },
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "EII",
                        ESMCode = 3
                    }
                };

            TestEmploymentStatusMonitoring[] previousEmploymentStatusMonitorings = null;

            NewRule().LearnerEmploymentStatusMonitoringConditionMet(
                previousEmploymentStatusMonitorings,
                employmentStatusMonitorings)
                .Should().BeFalse();
        }

        [Fact]
        public void LearnerEmploymentStatusMonitoringConditionMet_True()
        {
            var employmentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "LOE",
                        ESMCode = 2
                    },
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "EII",
                        ESMCode = 3
                    }
                };

            var previousEmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                {
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "loe",
                        ESMCode = 2
                    },
                    new TestEmploymentStatusMonitoring()
                    {
                        ESMType = "eii",
                        ESMCode = 3
                    }
                };

            NewRule().LearnerEmploymentStatusMonitoringConditionMet(
                previousEmploymentStatusMonitorings,
                employmentStatusMonitorings)
                .Should().BeTrue();
        }

        [Fact]
        public void LearnerEmploymentStatusMonitoringConditionMet_True_NullCheck()
        {
            TestEmploymentStatusMonitoring[] employmentStatusMonitorings = null;

            TestEmploymentStatusMonitoring[] previousEmploymentStatusMonitorings = null;

            NewRule().LearnerEmploymentStatusMonitoringConditionMet(
                previousEmploymentStatusMonitorings,
                employmentStatusMonitorings)
                .Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "1A05678",
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LOE",
                                ESMCode = 2
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "EII",
                                ESMCode = 3
                            }
                        }
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123a01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "loe",
                                ESMCode = 2
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "eii",
                                ESMCode = 3
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Error_NullCheck()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "1A05678",
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LOE",
                                ESMCode = 2
                            },
                            null,
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "EII",
                                ESMCode = 3
                            }
                        }
                    },
                    null,
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123a01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "loe",
                                ESMCode = 2
                            },
                            null,
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "eii",
                                ESMCode = 3
                            },
                            null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "1A05678",
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LOE",
                                ESMCode = 2
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "EII",
                                ESMCode = 3
                            }
                        }
                    },
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A02",
                        EmpIdNullable = 11769,
                        EmpStat = 12,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "ISB",
                                ESMCode = 6
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LHR",
                                ESMCode = 7
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearnerNullCheck()
        {
            TestLearner testLearner = null;

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_EmploymentStatusNullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearnerEmploymentStatuses = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_DataNullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearnRefNumber = "1A05678",
                LearnerEmploymentStatuses = new TestLearnerEmploymentStatus[]
                {
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A01",
                        EmpIdNullable = 11768,
                        EmpStat = 10,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LOE",
                                ESMCode = 2
                            },
                            null,
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "EII",
                                ESMCode = 3
                            }
                        }
                    },
                    null,
                    new TestLearnerEmploymentStatus()
                    {
                        AgreeId = "123A02",
                        EmpIdNullable = 11769,
                        EmpStat = 12,
                        EmploymentStatusMonitorings = new TestEmploymentStatusMonitoring[]
                        {
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "ISB",
                                ESMCode = 6
                            },
                            new TestEmploymentStatusMonitoring()
                            {
                                ESMType = "LHR",
                                ESMCode = 7
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("EmpStat", 10)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("DateEmpStatApp", "01/07/2018")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("EmpId", 11768)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("ESMType", "LOE")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("ESMCode", 3)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(10, new DateTime(2018, 07, 01), 11768, "LOE", 3);
            validationErrorHandlerMock.Verify();
        }

        public R97Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R97Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
