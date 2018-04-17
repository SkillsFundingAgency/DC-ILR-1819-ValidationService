using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.FundModel.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_01RuleTests : AbstractRuleTests<FundModel_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;

            var fundModelServiceMock = new Mock<IFundModelDataService>();

            fundModelServiceMock.Setup(ds => ds.Exists(fundModel)).Returns(false);

            NewRule(fundModelServiceMock.Object).ConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var fundModel = 1;

            var fundModelServiceMock = new Mock<IFundModelDataService>();

            fundModelServiceMock.Setup(ds => ds.Exists(fundModel)).Returns(true);

            NewRule(fundModelServiceMock.Object).ConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var fundModel = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel
                    }
                }
            };

            var fundModelServiceMock = new Mock<IFundModelDataService>();

            fundModelServiceMock.Setup(ds => ds.Exists(fundModel)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fundModelServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var fundModel = 1;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel
                    }
                }
            };

            var fundModelServiceMock = new Mock<IFundModelDataService>();

            fundModelServiceMock.Setup(ds => ds.Exists(fundModel)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fundModelServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private FundModel_01Rule NewRule(IFundModelDataService fundModelDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_01Rule(fundModelDataService, validationErrorHandler);
        }
    }
}
