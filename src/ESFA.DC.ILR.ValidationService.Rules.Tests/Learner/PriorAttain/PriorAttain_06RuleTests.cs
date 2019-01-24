using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_06RuleTests : AbstractRuleTests<PriorAttain_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_06");
        }

        [Theory]
        [InlineData(70, true)]
        [InlineData(25, false)]
        [InlineData(0, false)]
        public void FundModelConditionMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Fact]
        public void PriorAttainConditionMet_True()
        {
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void PriorAttainConditionMet_False()
        {
            int priorAttain = 9;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void PriorAttainConditionMet_MinMaxLevelsNull_ReturnsFalse()
        {
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns(string.Empty);
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns(string.Empty);
            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            NewRule(fcsDataService: fcsDataServiceMock.Object).ConditionMet(priorAttain, fundModel, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            int priorAttain = 9;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            NewRule(fcsDataService: fcsDataServiceMock.Object).ConditionMet(priorAttain, fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 3,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_OtherFundModels_NoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(f => f.GetMinPriorAttainment(conRefNumber)).Returns("1");
            fcsDataServiceMock.Setup(f => f.GetMaxPriorAttainment(conRefNumber)).Returns("3");
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NoLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, "1")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1, "1");

            validationErrorHandlerMock.Verify();
        }

        private PriorAttain_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IFCSDataService fcsDataService = null)
        {
            return new PriorAttain_06Rule(validationErrorHandler, fcsDataService);
        }
    }
}
