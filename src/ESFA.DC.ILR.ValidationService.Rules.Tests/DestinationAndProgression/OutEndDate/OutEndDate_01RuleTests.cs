using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutEndDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutEndDate
{
    public class OutEndDate_01RuleTests : AbstractRuleTests<OutEndDate_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutEndDate_01");
        }

        [Fact]
        public void Validate_Errors()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutStartDate = new DateTime(2018, 12, 12),
                        OutEndDateNullable = new DateTime(2018, 08, 01)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutStartDate = new DateTime(2018, 12, 01),
                        OutEndDateNullable = new DateTime(2018, 12, 12)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void Validate_Null_NoErrors()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutStartDate = new DateTime(2018, 12, 01),
                        OutEndDateNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        private OutEndDate_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutEndDate_01Rule(validationErrorHandler);
        }
    }
}
