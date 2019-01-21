using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SEC;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SEC
{
    public class SEC_02RuleTests : AbstractRuleTests<SEC_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SEC_02");
        }

        [Theory]
        [InlineData("XF")]
        [InlineData("XG")]
        [InlineData("XH")]
        [InlineData("XI")]
        [InlineData("XK")]
        public void ConditionMet_True(string domicile)
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var ucasAppId = "ABC";
            int? sec = null;

            NewRule().ConditionMet(learnStartDate, ucasAppId, domicile, sec).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThanFirstAugust2013()
        {
            var learnStartDate = new DateTime(2012, 01, 01);
            var ucasAppId = "ABC";
            var domicile = "XK";
            int? sec = null;

            NewRule().ConditionMet(learnStartDate, ucasAppId, domicile, sec).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_UcasAppId()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            string ucasAppId = null;
            var domicile = "XK";
            int? sec = null;

            NewRule().ConditionMet(learnStartDate, ucasAppId, domicile, sec).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Domicile()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var ucasAppId = "ABC";
            var domicile = "DEF";
            int? sec = null;

            NewRule().ConditionMet(learnStartDate, ucasAppId, domicile, sec).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_SecNotNull()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var ucasAppId = "ABC";
            var domicile = "XK";
            int? sec = 1;

            NewRule().ConditionMet(learnStartDate, ucasAppId, domicile, sec).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = "ABC",
                            DOMICILE = "XF",
                            SECNullable = null
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            UCASAPPID = "ABC",
                            DOMICILE = "XF",
                            SECNullable = 1
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnStartDate = new DateTime(2018, 01, 01);
            var domicile = "ABC";
            var ucasAppId = "DEF";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, domicile)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.UCASAPPID, ucasAppId)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, domicile, ucasAppId);

            validationErrorHandlerMock.Verify();
        }

        private SEC_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new SEC_02Rule(validationErrorHandler);
        }
    }
}
