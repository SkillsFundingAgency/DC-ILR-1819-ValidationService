using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.QUALENT3;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.QUALENT3
{
    public class QUALENT3_02RuleTests : AbstractRuleTests<QUALENT3_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("QUALENT3_02");
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                QUALENT3 = "AB1"
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False_NullEntity()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE();

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void QUALENT3ValidConditionMet_True()
        {
            var qualent3 = "AB1";

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(qualent3)).Returns(false);

            NewRule(qualent3DataServiceMock.Object).QUALENT3ValidConditionMet(qualent3).Should().BeTrue();
        }

        [Fact]
        public void QUALENT3ValidConditionMet_False()
        {
            var qualent3 = "AB1";

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(qualent3)).Returns(true);

            NewRule(qualent3DataServiceMock.Object).QUALENT3ValidConditionMet(qualent3).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                QUALENT3 = "AB1"
            };

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(learningDeliveryHE.QUALENT3)).Returns(false);

            NewRule(qualent3DataServiceMock.Object).ConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData("AB1")]
        [InlineData(null)]
        public void ConditionMet_False(string qualent3)
        {
            var learningDeliveryHE = new TestLearningDeliveryHE
            {
                QUALENT3 = qualent3
            };

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(learningDeliveryHE.QUALENT3)).Returns(true);

            NewRule(qualent3DataServiceMock.Object).ConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var qualent3 = "AB1";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE
                        {
                            QUALENT3 = qualent3
                        }
                    }
                }
            };

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(qualent3)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(qualent3DataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var qualent3 = "AB1";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE
                        {
                            QUALENT3 = qualent3
                        }
                    }
                }
            };

            var qualent3DataServiceMock = new Mock<IQUALENT3DataService>();

            qualent3DataServiceMock.Setup(ds => ds.Exists(qualent3)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(qualent3DataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("QUALENT3", "AB1")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("AB1");

            validationErrorHandlerMock.Verify();
        }

        private QUALENT3_02Rule NewRule(IQUALENT3DataService qualent3DataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new QUALENT3_02Rule(qualent3DataService, validationErrorHandler);
        }
    }
}
