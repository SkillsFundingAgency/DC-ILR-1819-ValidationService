using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_72RuleTests : AbstractRuleTests<LearnAimRef_72Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_72");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.AdultSkills).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.EuropeanSocialFund).Should().BeTrue();
        }

        [Theory]
        [InlineData(ValidationConstants.ZESF0001)]
        [InlineData("zesf0001")]
        public void LearnAimRefConditionMet_False(string learnAimRef)
        {
            NewRule().LearnAimRefConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LearnAimRefConditionMet_True()
        {
            NewRule().LearnAimRefConditionMet("ZESF00028").Should().BeTrue();
        }

        [Fact]
        public void FCSConditionMet_False()
        {
            string conRefNumber = "ZESF00024";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(false);

            NewRule(fCSDataService: fcsDataServiceMock.Object).FCSConditionMet(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FCSConditionMet_True()
        {
            string conRefNumber = "ZESF00025";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(true);

            NewRule(fCSDataService: fcsDataServiceMock.Object).FCSConditionMet(conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            string learnAimRef = "ESF123456";
            string conRefNumber = "ZESF00005";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(false);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fCSDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string learnAimRef = "ESF987654";
            string conRefNumber = "ZESF00099";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fcsDataServiceMock = new Mock<IFCSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(true);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                fCSDataService: fcsDataServiceMock.Object)
                .LARSConditionMet(conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            int fundModel = TypeOfFunding.AdultSkills;
            string conRefNumber = "ZESF00098";
            string learnAimRef = ValidationConstants.ZESF0001;

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(false);
            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(false);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(fundModel, conRefNumber, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            string conRefNumber = "ZESF00099";
            string learnAimRef = "ESF987654";

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(true);

            NewRule(
                fCSDataService: fcsDataServiceMock.Object,
                lARSDataService: larsDataServiceMock.Object)
                .ConditionMet(fundModel, conRefNumber, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            string conRefNumber = "ZESF00099";
            string learnAimRef = "ESF987654";

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(true);
            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fCSDataService: fcsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int fundModel = TypeOfFunding.AdultSkills;
            string conRefNumber = "ZESF00098";
            string learnAimRef = ValidationConstants.ZESF0001;

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.GetNotionalNVQLevelv2ForLearnAimRef(learnAimRef)).Returns("2");
            fcsDataServiceMock.Setup(f => f.IsNotionalNVQLevel2BetweenSubjectAreaMinMaxValues(2, conRefNumber)).Returns(false);
            fcsDataServiceMock.Setup(s => s.IsSectorSubjectAreaCodeNullForContract(conRefNumber)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fCSDataService: fcsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(null);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDelivery_NullCheck()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = null
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.EuropeanSocialFund)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, "ESF00012")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(TypeOfFunding.EuropeanSocialFund, "ESF00012");

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_72Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFCSDataService fCSDataService = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearnAimRef_72Rule(
                validationErrorHandler: validationErrorHandler,
                fCSDataService: fCSDataService,
                lARSDataService: lARSDataService);
        }
    }
}
