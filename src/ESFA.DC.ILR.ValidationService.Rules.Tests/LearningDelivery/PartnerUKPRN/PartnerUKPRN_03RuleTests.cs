using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_03RuleTests : AbstractRuleTests<PartnerUKPRN_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PartnerUKPRN_03");
        }

        [Fact]
        public void NullConditionMet_True()
        {
            NewRule().NullConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void NullConditionMet_False()
        {
            NewRule().NullConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void UKPRNConditionMet_False()
        {
            NewRule().UKPRNConditionMet(1, 2).Should().BeFalse();
        }

        [Fact]
        public void UKPRNConditionMet_True()
        {
            NewRule().UKPRNConditionMet(1, 1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(1, 2).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1, 1).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullUkprn()
        {
            NewRule().ConditionMet(1, null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        PartnerUKPRNNullable = 1
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                    {
                        PartnerUKPRNNullable = 1
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(2);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            long ukprn = 1;
            long? partnerUKPRN = 1;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("UKPRN", ukprn)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PartnerUKPRN", partnerUKPRN)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(ukprn, partnerUKPRN);

            validationErrorHandlerMock.Verify();
        }

        private PartnerUKPRN_03Rule NewRule(IFileDataService fileDatService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PartnerUKPRN_03Rule(fileDatService, validationErrorHandler);
        }
    }
}
