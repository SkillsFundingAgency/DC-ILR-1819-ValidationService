﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67RuleTests : AbstractRuleTests<LearnDelFAMType_67Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_67");
        }

        [Fact]
        public void ValidationPassesIrrelevantFamType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationPassesBasicSkill()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(true);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 3
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationPasses_IrrelevantFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 3,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationPasses_IrrelevantAimType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 13,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(m => m.BasicSkillsMatchForLearnAimRefAndStartDate(It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(false);

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 36,
                        AimType = 3,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<string>()));
        }

        private LearnDelFAMType_67Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILARSDataService larsDataService = null)
        {
            return new LearnDelFAMType_67Rule(validationErrorHandler, larsDataService);
        }
    }
}
