using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_64RuleTests : AbstractRuleTests<LearnDelFAMType_64Rule>
    {
        private readonly IEnumerable<int> _basicSkillsTypes = new List<int>() { 01, 11, 13, 20, 23, 24, 29, 31, 02, 12, 14, 19, 21, 25, 30, 32, 33, 34, 35 };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_64");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(FundModelConstants.AdultSkills).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(FundModelConstants.Apprenticeships).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, "00100310", "2017-01-01")]
        [InlineData(3, "00100310", "2017-01-01")]
        [InlineData(3, "00100309", "2017-01-01")]
        [InlineData(5, "00100310", "2018-06-01")]
        public void AimTypeConditionMet_False(int aimType, string learnAimRef, string learnStartDateString)
        {
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).AimTypeConditionMet(aimType, learnAimRef, learnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, "00100309", "2018-06-01")]
        [InlineData(3, "00100309", "2018-06-01")]
        public void AimTypeConditionMet_True(int aimType, string learnAimRef, string learnStartDateString)
        {
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).AimTypeConditionMet(aimType, learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LarsConditionMet_False()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).LarsConditionMet("00100310", new DateTime(2017, 01, 01)).Should().BeFalse();
        }

        [Fact]
        public void LarsConditionMet_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).LarsConditionMet("00100309", new DateTime(2018, 06, 01)).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModelConstants.AdultSkills, 2, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(FundModelConstants.Apprenticeships, 2, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(FundModelConstants.Apprenticeships, 1, LearningDeliveryFAMTypeConstants.ACT, "00100310", "2017-01-01")]
        [InlineData(FundModelConstants.Apprenticeships, 1, LearningDeliveryFAMTypeConstants.ADL, "00100310", "2017-01-01")]
        [InlineData(FundModelConstants.Apprenticeships, 1, LearningDeliveryFAMTypeConstants.ADL, "00100309", "2017-01-01")]
        public void ConditionMet_False(int fundModel, int aimType, string learnDelFAMType, string learnAimRef, string learnStartDateString)
        {
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(true);

            NewRule(
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object).ConditionMet(fundModel, aimType, learningDeliveryFAMs, learnAimRef, learnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModelConstants.Apprenticeships, 1, LearningDeliveryFAMTypeConstants.ADL, "00100309", "2018-06-01")]
        [InlineData(FundModelConstants.Apprenticeships, 3, LearningDeliveryFAMTypeConstants.LDM, "00100309", "2018-06-01")]
        public void ConditionMet_True(int fundModel, int aimType, string learnDelFAMType, string learnAimRef, string learnStartDateString)
        {
            DateTime.TryParse(learnStartDateString, out DateTime learnStartDate);

            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType },
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(true);

            NewRule(
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object).ConditionMet(fundModel, aimType, learningDeliveryFAMs, learnAimRef, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL },
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "00100309",
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 06, 01),
                        FundModel = FundModelConstants.Apprenticeships,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100309", new DateTime(2018, 6, 1))).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "00100309",
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 06, 01),
                        FundModel = FundModelConstants.Apprenticeships,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(true);
            larsDataServiceMock.Setup(ldc => ldc.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsTypes, "00100310", new DateTime(2017, 1, 1))).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(learner);
            }
        }

        public LearnDelFAMType_64Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearnDelFAMType_64Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                lARSDataService: lARSDataService);
        }
    }
}
