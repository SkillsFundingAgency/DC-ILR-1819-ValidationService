using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.PCSLDCS;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.PCSLDCS
{
    public class PCSLDCS_02RuleTests : AbstractRuleTests<PCSLDCS_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PCSLDCS_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            decimal? pcsldcs = 10.5m;
            string learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, pcsldcs).Should().BeTrue();
        }

        [Theory]
        [InlineData("10.5", true)]
        [InlineData(null, false)]
        public void ConditionMet_False(string pcsldcs, bool larsDataMockResult)
        {
            decimal? pcsldcsParam = string.IsNullOrEmpty(pcsldcs) ? (decimal?)null : decimal.Parse(pcsldcs);
            string learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef))
                .Returns(larsDataMockResult);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnAimRef, pcsldcsParam).Should().BeFalse();
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
                            PCSLDCSNullable = 10.5m
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef))
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
                            PCSLDCSNullable = 10.5m
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ldsm => ldsm.LearnDirectClassSystemCode2MatchForLearnAimRef(learnAimRef))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            decimal? pcsldcs = 32.5m;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PCSLDCS, pcsldcs)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pcsldcs);

            validationErrorHandlerMock.Verify();
        }

        private PCSLDCS_02Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PCSLDCS_02Rule(larsDataService, validationErrorHandler);
        }
    }
}