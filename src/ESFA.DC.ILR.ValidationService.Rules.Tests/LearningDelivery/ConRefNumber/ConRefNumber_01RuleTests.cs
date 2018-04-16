using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ConRefNumber
{
    public class ConRefNumber_01RuleTests : AbstractRuleTests<ConRefNumber_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ConRefNumber_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(false);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(true);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ConRefNumber()
        {
            const int fundModel = 1;
            const string conRefNumber = "abc";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(r => r.ConRefNumberConditionMet(conRefNumber)).Returns(false);

            ruleMock.Object.ConditionMet(fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(70).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(71).Should().BeFalse();
        }

        [Fact]
        public void ConRefNumberConditionMet_True()
        {
            var conRefNumber = "abc";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.ConRefNumberExists(conRefNumber)).Returns(false);

            NewRule(fcsDataServiceMock.Object).ConRefNumberConditionMet(conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void ConRefNumberConditionMet_True_Null()
        {
            NewRule().ConRefNumberConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void ConRefNumberConditionMet_True_WhiteSpace()
        {
            NewRule().ConRefNumberConditionMet("  ").Should().BeTrue();
        }

        [Fact]
        public void ConRefNumberConditionMet_False_Lookup()
        {
            var conRefNumber = "abc";

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.ConRefNumberExists(conRefNumber)).Returns(true);

            NewRule(fcsDataServiceMock.Object).ConRefNumberConditionMet(conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var conRefNumber = "abc";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        ConRefNumber = conRefNumber,
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.ConRefNumberExists(conRefNumber)).Returns(false);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fcsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var conRefNumber = "abc";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 70,
                        ConRefNumber = conRefNumber,
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();

            fcsDataServiceMock.Setup(ds => ds.ConRefNumberExists(conRefNumber)).Returns(true);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fcsDataServiceMock.Object, validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ConRefNumber", "abc")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, "abc");

            validationErrorHandlerMock.Verify();
        }

        private ConRefNumber_01Rule NewRule(IFCSDataService fcsDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ConRefNumber_01Rule(fcsDataService, validationErrorHandler);
        }
    }
}
