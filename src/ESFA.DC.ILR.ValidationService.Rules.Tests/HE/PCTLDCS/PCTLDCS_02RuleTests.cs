using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCTLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCTLDCS
{
    public class PCTLDCS_02RuleTests : AbstractRuleTests<PCTLDCS_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCTLDCS_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            decimal? pctldcs = 10.5m;
            string learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, pctldcs).Should().BeTrue();
        }

        [Theory]
        [InlineData("10.5", true)]
        [InlineData(null, false)]
        public void ConditionMet_False(string pctldcs, bool larsDataMockResult)
        {
            decimal? pctldcsParam = string.IsNullOrEmpty(pctldcs) ? (decimal?)null : decimal.Parse(pctldcs);

            string learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(larsDataMockResult);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, pctldcsParam).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learnAimRef = "123456789";

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCTLDCSNullable = 10.5m
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnAimRef = "123456789";

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            PCTLDCSNullable = 10.5m
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            decimal? pctldcs = 32.5m;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCTLDCS, pctldcs)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pctldcs);

            validationErrorHandlerMock.Verify();
        }

        private PCTLDCS_02Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCTLDCS_02Rule(larsDataService, validationErrorHandler);
        }
    }
}
