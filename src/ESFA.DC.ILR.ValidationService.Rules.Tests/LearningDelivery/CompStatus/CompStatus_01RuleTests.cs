using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.CompStatus
{
    public class CompStatus_01RuleTests : AbstractRuleTests
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var compStatus = 1;

            var compStatusInternalDataServiceMock = new Mock<ICompStatusDataService>();

            compStatusInternalDataServiceMock.Setup(ds => ds.Exists(compStatus)).Returns(false);

            NewRule(compStatusInternalDataServiceMock.Object).ConditionMet(compStatus).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var compStatus = 1;

            var compStatusInternalDataServiceMock = new Mock<ICompStatusDataService>();

            compStatusInternalDataServiceMock.Setup(ds => ds.Exists(compStatus)).Returns(true);

            NewRule(compStatusInternalDataServiceMock.Object).ConditionMet(compStatus).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var compStatus = 1;

            var learningDelivery = new TestLearningDelivery()
            {
                CompStatus = compStatus
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var compStatusInternalDataServiceMock = new Mock<ICompStatusDataService>();

            compStatusInternalDataServiceMock.Setup(ds => ds.Exists(compStatus)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(compStatusInternalDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var compStatus = 1;

            var learningDelivery = new TestLearningDelivery()
            {
                CompStatus = compStatus
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var compStatusInternalDataServiceMock = new Mock<ICompStatusDataService>();

            compStatusInternalDataServiceMock.Setup(ds => ds.Exists(compStatus)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(compStatusInternalDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private CompStatus_01Rule NewRule(ICompStatusDataService compStatusInternalDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_01Rule(compStatusInternalDataService, validationErrorHandler);
        }
    }
}
