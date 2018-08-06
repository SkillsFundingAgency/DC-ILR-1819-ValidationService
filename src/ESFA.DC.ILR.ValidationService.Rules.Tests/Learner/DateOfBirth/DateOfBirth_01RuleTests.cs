using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_01RuleTests : AbstractRuleTests<DateOfBirth_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_01");
        }

        [Theory]
        [InlineData(25)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(70)]
        [InlineData(81)]
        [InlineData(82)]
        public void ConditionMet_True(int fundModel)
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(fundModel, null, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DateOfBirth()
        {
            NewRule().ConditionMet(25, null, new DateTime(1988, 12, 25)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(1, null, new DateTime(1988, 12, 25)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMType()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(10, null, null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1988, 12, 25),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();

            NewRule(null, validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_01Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_01Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
