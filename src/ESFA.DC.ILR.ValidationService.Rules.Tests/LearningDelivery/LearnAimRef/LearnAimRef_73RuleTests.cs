using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_73RuleTests : AbstractRuleTests<LearnAimRef_73Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_73");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.Other16To19).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.EuropeanSocialFund).Should().BeTrue();
        }

        [Fact]
        public void FCSConditionMet_False()
        {
            string conRefNumber = "ZESF123456";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(false);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCSConditionMet(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FCSConditionMet_True()
        {
            string conRefNumber = "ZESF987654";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(true);

            NewRule(fcsDataService: fcsDataServiceMock.Object).FCSConditionMet(conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void LearnAimRefConditionMet_False()
        {
            NewRule().LearnAimRefConditionMet(ValidationConstants.ZESF0001).Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefConditionMet_True()
        {
            NewRule().LearnAimRefConditionMet("ZESF0002").Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            string conRefNumber = "ZESF123456";
            string learnAimRef = "1234567";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(false);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fcsDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_False_EmptyValueCheck()
        {
            string conRefNumber = "ZESF123456";
            string learnAimRef = "1234567";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                2, conRefNumber)).Returns(false);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fcsDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_False_NullCheck()
        {
            string conRefNumber = "ZESF123456";
            string learnAimRef = "1234567";

            ILARSLearningDelivery lARSLearningDelivery = null;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                1.2M,
                2.3M)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                2,
                conRefNumber)).Returns(false);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fcsDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string conRefNumber = "ZESF123456";
            string learnAimRef = "1234567";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(false);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(true);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fcsDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.Other16To19, "ZESF123456", ValidationConstants.ZESF0001)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, "ZESF123456", ValidationConstants.ZESF0001)]
        [InlineData(TypeOfFunding.Other16To19, "ZESF123456", "ZESF0002")]
        public void ConditionMet_False(int fundModel, string conRefNumber, string learnAimRef)
        {
            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(false);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(false);

            NewRule(
                fcsDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(fundModel, conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            string conRefNumber = "ZESF987654";
            string learnAimRef = "ZESF0002";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(false);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(true);

            NewRule(
                fcsDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(fundModel, conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            string conRefNumber = "ZESF987654";
            string learnAimRef = "ZESF0002";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(false);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fcsDataService: fcsDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            string conRefNumber = "123456";
            string learnAimRef = "ZESF0001";

            ILARSLearningDelivery lARSLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                SectorSubjectAreaTier1 = 1.2M,
                SectorSubjectAreaTier2 = 2.3M,
                NotionalNVQLevelv2 = "2"
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ConRefNumber = conRefNumber,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetLearningDeliveryForLearnAimRef(learnAimRef)).Returns(lARSLearningDelivery);
            fcsDataServiceMock.Setup(f => f.IsSubjectAreaAndMinMaxLevelsExistsForContract(conRefNumber)).Returns(false);
            fcsDataServiceMock.Setup(f => f.IsSectorSubjectAreaTiersMatchingSubjectAreaCode(
                conRefNumber,
                lARSLearningDelivery.SectorSubjectAreaTier1,
                lARSLearningDelivery.SectorSubjectAreaTier2)).Returns(true);
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(
                int.Parse(lARSLearningDelivery.NotionalNVQLevelv2),
                conRefNumber)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fcsDataService: fcsDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, "ZESF321456")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, "ZESF321456");

            validationErrorHandlerMock.Verify();
        }

        public LearnAimRef_73Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFCSDataService fcsDataService = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearnAimRef_73Rule(
                validationErrorHandler: validationErrorHandler,
                fCSDataService: fcsDataService,
                lARSDataService: lARSDataService);
        }
    }
}
