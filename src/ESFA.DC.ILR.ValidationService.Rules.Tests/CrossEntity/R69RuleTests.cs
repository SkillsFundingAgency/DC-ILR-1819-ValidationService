using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R69RuleTests : AbstractRuleTests<R71Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R69");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345",
                        DPOutcomes = new IDPOutcome[]
                        {
                            new TestDPOutcome()
                            {
                                OutCode = 1,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestDPOutcome()
                            {
                                OutCode = 2,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                        }
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "123456",
                        DPOutcomes = new IDPOutcome[]
                        {
                            new TestDPOutcome()
                            {
                                OutCode = 1,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestDPOutcome()
                            {
                                OutCode = 2,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345",
                        DPOutcomes = new IDPOutcome[]
                        {
                            new TestDPOutcome()
                            {
                                OutCode = 1,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestDPOutcome()
                            {
                                OutCode = 1,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                        }
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "123456",
                        DPOutcomes = new IDPOutcome[]
                        {
                            new TestDPOutcome()
                            {
                                OutCode = 1,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                            new TestDPOutcome()
                            {
                                OutCode = 2,
                                OutType = "A",
                                OutStartDate = new DateTime(2018, 10, 10)
                            },
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
            validationErrorHandlerMock.Verify(h => h.Handle("R69", "12345", null, null));
            validationErrorHandlerMock.Verify(h => h.Handle("R69", "123456", null, null), Times.Never);
        }

        private R69Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R69Rule(validationErrorHandler);
        }
    }
}
