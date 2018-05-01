using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_80RuleTests : AbstractRuleTests<LearnAimRef_80Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_80");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LevelConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.OrganisationConditionMet()).Returns(true);
            ruleMock.Setup(r => r.RestartConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PriorAttain()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Level()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LevelConditionMet(learnAimRef)).Returns(false);
            ruleMock.Setup(r => r.OrganisationConditionMet()).Returns(true);
            ruleMock.Setup(r => r.RestartConditionMet(learningDeliveryFams)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Organisation()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LevelConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.OrganisationConditionMet()).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Restart()
        {
            var fundModel = 1;
            var priorAttain = 1;
            var learnStartDate = new DateTime(2018, 1, 1);
            var progType = 1;
            var learnAimRef = "LearnAimRef";
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.PriorAttainmentConditionMet(priorAttain)).Returns(true);
            ruleMock.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(r => r.ApprenticeshipConditionMet(progType)).Returns(true);
            ruleMock.Setup(r => r.LevelConditionMet(learnAimRef)).Returns(true);
            ruleMock.Setup(r => r.OrganisationConditionMet()).Returns(true);
            ruleMock.Setup(r => r.RestartConditionMet(learningDeliveryFams)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, priorAttain, learnStartDate, progType, learnAimRef, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2017, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False_Before()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2015, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_False_After()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(36).Should().BeFalse();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(97)]
        [InlineData(98)]
        public void PriorAttainmentConditionMet_True(int priorAttain)
        {
            NewRule().PriorAttainmentConditionMet(priorAttain).Should().BeTrue();
        }

        [Fact]
        public void PriorAttainmentConditionMet_False()
        {
            NewRule().PriorAttainmentConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void LevelConditionMet_True()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRef(learnAimRef, "3")).Returns(true);

            NewRule(larsDataServiceMock.Object).LevelConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LevelConditionMet_False()
        {
            var learnAimRef = "LearnAimRef";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRef(learnAimRef, "3")).Returns(false);

            NewRule(larsDataServiceMock.Object).LevelConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False()
        {
            var progType = 1;

            var dd07Mock = new Mock<IDD07>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void RestartConditionMet_True()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).RestartConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void RestartConditionMet_False()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).RestartConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void OrganisationConditionMet_True()
        {
            var ukprn = 1;

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, "USDC")).Returns(false);

            NewRule(organisationDataService: organisationDataServiceMock.Object, fileDataCache: fileDataCacheMock.Object).OrganisationConditionMet().Should().BeTrue();
        }

        [Fact]
        public void OrganisationConditionMet_False()
        {
            var ukprn = 1;

            var fileDataCacheMock = new Mock<IFileDataCache>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();

            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, "USDC")).Returns(true);

            NewRule(organisationDataService: organisationDataServiceMock.Object, fileDataCache: fileDataCacheMock.Object).OrganisationConditionMet().Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "LearnAimRef";
            var progType = 1;
            var ukprn = 1;
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learner = new TestLearner()
            {
                PriorAttainNullable = 3,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2017, 1, 1),
                        FundModel = 35,
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFams,
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDD07>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileDataCacheMock = new Mock<IFileDataCache>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            larsDataServiceMock.Setup(ds => ds.NotionalNVQLevelV2MatchForLearnAimRef(learnAimRef, "3")).Returns(true);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            fileDataCacheMock.SetupGet(c => c.UKPRN).Returns(ukprn);
            organisationDataServiceMock.Setup(ds => ds.LegalOrgTypeMatchForUkprn(ukprn, "USDC")).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    larsDataServiceMock.Object,
                    organisationDataServiceMock.Object,
                    learningDeliveryFamQueryServiceMock.Object,
                    dd07Mock.Object,
                    fileDataCacheMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2011, 1, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var priorAttain = 1;
            var learnAimRef = "LearnAimRef";
            var learnStartDate = new DateTime(2017, 1, 1);
            var fundModel = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PriorAttain", priorAttain)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(priorAttain, learnAimRef, learnStartDate, fundModel);

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_80Rule NewRule(
            ILARSDataService larsDataService = null,
            IOrganisationDataService organisationDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IDD07 dd07 = null,
            IFileDataCache fileDataCache = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_80Rule(
                larsDataService,
                organisationDataService,
                learningDeliveryFamQueryService,
                dd07,
                fileDataCache,
                validationErrorHandler);
        }
    }
}
