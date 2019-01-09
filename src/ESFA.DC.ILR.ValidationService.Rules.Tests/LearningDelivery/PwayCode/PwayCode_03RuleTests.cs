using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PwayCode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PwayCode
{
    public class PwayCode_03RuleTests : AbstractRuleTests<PwayCode_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PwayCode_03");
        }

        [Fact]
        public void Excluded_True()
        {
            var progType = 25;

            NewRule().Excluded(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(10)]
        public void Excluded_False(int? progType)
        {
            NewRule().Excluded(progType).Should().BeFalse();
        }

        [Fact]
        public void PwayCodeConditionMet_True()
        {
            int? pwayCode = null;

            NewRule().PwayCodeConditionMet(pwayCode).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void PwayCodeConditionMet_False(int? pwayCode)
        {
            NewRule().PwayCodeConditionMet(pwayCode).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 2;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 50;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int? pwayCode = null;
            var progType = 2;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Excluded()
        {
            int? pwayCode = null;
            var progType = 25;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PwayCode()
        {
            int? pwayCode = 1;
            var progType = 2;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD07()
        {
            int? pwayCode = null;
            var progType = 50;

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).ConditionMet(pwayCode, progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int? pwayCode = null;
            var progType = 2;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        PwayCodeNullable = pwayCode,
                        ProgTypeNullable = progType
                    },
                }
            };

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(null, 25, true)]
        [InlineData(null, 50, false)]
        public void Validate_NoError(int? pwayCode, int? progType, bool mockResult)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery
                    {
                        PwayCodeNullable = pwayCode,
                        ProgTypeNullable = progType
                    },
                }
            };

            var dd07Mock = new Mock<IDD07>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(mockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var pwayCode = 1;
            var progType = 2;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PwayCode", pwayCode)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pwayCode, progType);

            validationErrorHandlerMock.Verify();
        }

        private PwayCode_03Rule NewRule(
            IDD07 dd07 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new PwayCode_03Rule(dd07, validationErrorHandler);
        }
    }
}
