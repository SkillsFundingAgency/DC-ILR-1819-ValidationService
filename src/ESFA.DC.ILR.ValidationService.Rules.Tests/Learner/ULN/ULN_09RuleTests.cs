using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_09RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(true, new DateTime(2018, 2, 1), new DateTime(2018, 1, 1), 9999999999).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FamTypeCode()
        {
            NewRule().ConditionMet(false, new DateTime(2018, 2, 1), new DateTime(2018, 1, 1), 9999999999).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Uln()
        {
            NewRule().ConditionMet(true, new DateTime(2018, 2, 1), new DateTime(2018, 1, 1), 999999999).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Dates()
        {
            NewRule().ConditionMet(true, new DateTime(2018, 1, 1), new DateTime(2018, 2, 1), 999999999).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 1,
                LearningDeliveries = new TestLearningDelivery[] { }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataServiceMock.SetupGet(fds => fds.FilePreparationDate).Returns(new DateTime(2017, 1, 1));
            validationDataServiceMock.SetupGet(vds => vds.AcademicYearJanuaryFirst).Returns(new DateTime(2017, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);

            var rule = new ULN_09Rule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, null);

            rule.Validate(learner);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataServiceMock.SetupGet(fds => fds.FilePreparationDate).Returns(new DateTime(2017, 1, 1));
            validationDataServiceMock.SetupGet(vds => vds.AcademicYearJanuaryFirst).Returns(new DateTime(2017, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_09", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new ULN_09Rule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private ULN_09Rule NewRule(IFileDataService fileDataService = null, IValidationDataService validationDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_09Rule(fileDataService, validationDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
