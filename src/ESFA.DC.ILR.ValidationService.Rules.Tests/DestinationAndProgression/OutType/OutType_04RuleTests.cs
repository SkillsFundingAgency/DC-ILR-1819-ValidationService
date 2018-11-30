using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutType
{
    public class OutType_04RuleTests : AbstractRuleTests<OutType_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutType_04");
        }

        [Fact]
        public void OutTypesConditionMet_True()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().OutTypesConditionMet(dpOutcomes).Should().BeTrue();
        }

        [Fact]
        public void OutTypesConditionMet_FalseNoEMP()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().OutTypesConditionMet(dpOutcomes).Should().BeFalse();
        }

        [Fact]
        public void OutTypesConditionMet_FalseNoNPE()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().OutTypesConditionMet(dpOutcomes).Should().BeFalse();
        }

        [Fact]
        public void OutTypesConditionMet_FalseNull()
        {
            NewRule().OutTypesConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void OutStartDateConditionMet_True()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2018, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2019, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().OutStartDateConditionMet(dpOutcomes).Should().BeTrue();
        }

        [Fact]
        public void OutStartDateConditionMet_FalseNoMatch()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2018, 01, 01)
                },
            };

            NewRule().OutStartDateConditionMet(dpOutcomes).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2018, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2019, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().ConditionMet(dpOutcomes).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseOutTypes()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2018, 01, 01)
                },
            };

            NewRule().ConditionMet(dpOutcomes).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseOutStartDate()
        {
            var dpOutcomes = new List<TestDPOutcome>()
            {
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2018, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "NPE",
                    OutStartDate = new DateTime(2019, 01, 01)
                },
                new TestDPOutcome()
                {
                    OutType = "EMP",
                    OutStartDate = new DateTime(2017, 01, 01)
                },
            };

            NewRule().ConditionMet(dpOutcomes).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
                {
                    new TestDPOutcome()
                    {
                        OutType = "EMP",
                        OutStartDate = new DateTime(2017, 01, 01)
                    },
                    new TestDPOutcome()
                    {
                        OutType = "NPE",
                        OutStartDate = new DateTime(2017, 01, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
                {
                    new TestDPOutcome()
                    {
                        OutType = "EMP",
                        OutStartDate = new DateTime(2017, 01, 01)
                    },
                    new TestDPOutcome()
                    {
                        OutType = "NPE",
                        OutStartDate = new DateTime(2018, 01, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters_Multiple()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutStartDate", "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutType", "NPE, EMP")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1), "NPE, EMP");

            validationErrorHandlerMock.Verify();
        }

        private OutType_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutType_04Rule(validationErrorHandler);
        }
    }
}
