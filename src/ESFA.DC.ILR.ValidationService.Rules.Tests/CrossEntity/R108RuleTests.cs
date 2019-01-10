using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R108RuleTests : AbstractRuleTests<R108Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R108");
        }

        [Theory]
        [InlineData(24)]
        [InlineData(25)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(12)]
        [InlineData(null)]
        public void ProgTypeConditionMet_True(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModelConstants.NonFunded)]
        [InlineData(FundModelConstants.Apprenticeships)]
        public void FundModelConditionMet_False(int fundModel)
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { FundModel = fundModel }
            };
            NewRule().FundModelConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModelConstants.CommunityLearning)]
        [InlineData(FundModelConstants.AdultSkills)]
        [InlineData(FundModelConstants.ESF)]
        [InlineData(FundModelConstants.OtherAdult)]
        public void FundModelConditionMet_True(int fundModel)
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { FundModel = fundModel }
            };
            NewRule().FundModelConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void AllAimsClosedConditionMet_False()
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = null },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 06, 01) }
            };
            NewRule().AllAimsClosedConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void AllAimsClosedConditionMet_True()
        {
            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2017, 05, 01) },
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2018, 06, 01) }
            };
            NewRule().AllAimsClosedConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            DateTime? learnActEndDate = null;
            DateTime learnActEndDateExpected = new DateTime(2018, 06, 01);

            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = null, CompStatus = 5 },
                new TestLearningDelivery() { LearnActEndDateNullable = learnActEndDateExpected, CompStatus = 6 }
            };
            NewRule().CompStatusConditionMet(learningDeliveries, out learnActEndDate).Should().BeFalse();
            learnActEndDate.Should().Be(learnActEndDateExpected);
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            DateTime? learnActEndDate = null;
            DateTime learnActEndDateExpected = new DateTime(2018, 06, 01);

            IReadOnlyCollection<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { LearnActEndDateNullable = new DateTime(2017, 05, 02), CompStatus = 5 },
                new TestLearningDelivery() { LearnActEndDateNullable = learnActEndDateExpected }
            };
            NewRule().CompStatusConditionMet(learningDeliveries, out learnActEndDate).Should().BeTrue();
            learnActEndDate.Should().Be(learnActEndDateExpected);
        }

        [Fact]
        public void FilePreparationDateConditionMet_False()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(new DateTime(2018, 03, 01));
            NewRule(fileDataService: fileDataServiceMock.Object).FilePreparationDateConditionMet(new DateTime(2018, 06, 01)).Should().BeFalse();
        }

        [Fact]
        public void FilePreparationDateConditionMet_True()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(DateTime.Now.AddMonths(4));
            NewRule(fileDataService: fileDataServiceMock.Object).FilePreparationDateConditionMet(DateTime.Now).Should().BeTrue();
        }

        [Fact]
        public void DPOutComeConditionMet_False()
        {
            string learnRefNumber = "00100310";
            string ldapLearnRefNumber = string.Empty;

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100310",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2018, 09, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            NewRule().DPOutComeConditionMet(
                learnRefNumber,
                new List<TestLearnerDestinationAndProgression> { learnerDestinationAndProgression },
                new DateTime(2018, 06, 01),
                out ldapLearnRefNumber).Should().BeFalse();
            ldapLearnRefNumber.Should().BeEmpty();
        }

        [Theory]
        [InlineData("00100309")]
        [InlineData("00100310")]
        public void DPOutComeConditionMet_True(string learnRefNumber)
        {
            string ldapLearnRefNumber = string.Empty;

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100309",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2017, 05, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            string ldapLearnRefNumberExpectedValue =
                learnRefNumber == learnerDestinationAndProgression.LearnRefNumber
                ? learnRefNumber
                : string.Empty;

            NewRule()
                .DPOutComeConditionMet(
                learnRefNumber,
                new List<TestLearnerDestinationAndProgression> { learnerDestinationAndProgression },
                new DateTime(2018, 06, 01),
                out ldapLearnRefNumber).Should().BeTrue();
            ldapLearnRefNumber.Should().Be(ldapLearnRefNumberExpectedValue);
        }

        [Fact]
        public void DPOutComeConditionMet_True_NullCheck()
        {
            string learnRefNumber = "00100309";
            string ldapLearnRefNumber = string.Empty;

            List<TestLearnerDestinationAndProgression> learnerDestinationAndProgression = null;

            NewRule()
                .DPOutComeConditionMet(
                learnRefNumber,
                learnerDestinationAndProgression,
                new DateTime(2018, 06, 01),
                out ldapLearnRefNumber).Should().BeTrue();
            ldapLearnRefNumber.Should().BeEmpty();
        }

        [Fact]
        public void Validate_Error()
        {
            var learners = new TestLearner[]
            {
                new TestLearner()
                {
                    LearnRefNumber = "00100308",
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = 24,
                            FundModel = FundModelConstants.AdultSkills,
                            LearnActEndDateNullable = new DateTime(2017, 05, 01),
                            CompStatus = 7
                        },
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = null,
                            FundModel = FundModelConstants.ESF,
                            LearnActEndDateNullable = new DateTime(2018, 06, 01),
                            CompStatus = 8
                        }
                    }
                },
                new TestLearner()
                {
                    LearnRefNumber = "00100309",
                    LearningDeliveries = new TestLearningDelivery[]
                    {
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = 12,
                            FundModel = FundModelConstants.AdultSkills,
                            LearnActEndDateNullable = new DateTime(2017, 05, 01),
                            CompStatus = 7
                        },
                        new TestLearningDelivery()
                        {
                            ProgTypeNullable = null,
                            FundModel = FundModelConstants.ESF,
                            LearnActEndDateNullable = new DateTime(2018, 06, 01),
                            CompStatus = 8
                        }
                    }
                }
            };

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100309",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2017, 05, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            var message = new TestMessage()
            {
                Learners = learners,
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[]
                {
                    learnerDestinationAndProgression
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(DateTime.Now.AddMonths(4));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 24,
                        FundModel = FundModelConstants.NonFunded,
                        LearnActEndDateNullable = null,
                        CompStatus = 5
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 25,
                        FundModel = FundModelConstants.Apprenticeships,
                        LearnActEndDateNullable = new DateTime(2018, 06, 01),
                        CompStatus = 6
                    }
                }
            };

            var learnerDestinationAndProgression = new TestLearnerDestinationAndProgression()
            {
                LearnRefNumber = "00100310",
                DPOutcomes = new[]
                {
                    new TestDPOutcome() { OutStartDate = new DateTime(2018, 09, 01) },
                    new TestDPOutcome() { OutStartDate = new DateTime(2012, 01, 01) }
                }
            };

            var message = new TestMessage()
            {
                Learners = new TestLearner[] { learner },
                LearnerDestinationAndProgressions = new TestLearnerDestinationAndProgression[] { learnerDestinationAndProgression }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(fds => fds.FilePreparationDate()).Returns(new DateTime(2018, 03, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModelConstants.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, 5)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "01/06/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, "00100309")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(FundModelConstants.AdultSkills, 5, new DateTime(2018, 06, 01), "00100309");

            validationErrorHandlerMock.Verify();
        }

        public R108Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null)
        {
            return new R108Rule(validationErrorHandler: validationErrorHandler, fileDataService: fileDataService);
        }
    }
}
