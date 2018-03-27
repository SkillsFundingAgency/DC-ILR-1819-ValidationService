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
    public class ULN_03RuleTests
    {
        [Fact]
        public void Exclude_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "ACT", "1")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "ACT", "1")).Returns(false);

            var rule = new ULN_03Rule(null, null, learningDeliveryFAMQueryServiceMock.Object, null);

            rule.Exclude(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(70)]
        public void ConditionMet_True(long fundModel)
        {
            var rule = new ULN_03Rule(null, null, null, null);

            rule.ConditionMet(fundModel, 9999999999, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Uln()
        {
            var rule = new ULN_03Rule(null, null, null, null);

            rule.ConditionMet(25, 1, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var rule = new ULN_03Rule(null, null, null, null);

            rule.ConditionMet(1, 9999999999, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FilePreparationDate()
        {
            var rule = new ULN_03Rule(null, null, null, null);

            rule.ConditionMet(25, 9999999999, new DateTime(2030, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            var messageLearner = new TestLearner()
            {
                ULNNullable = 1,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 2,
                    }
                }
            };

            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));
            validationDataServiceMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(false);

            var rule = new ULN_03Rule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(messageLearner);
        }

        [Fact]
        public void Validate_Errors()
        {
            var messageLearner = new ILR.Tests.Model.TestLearner()
            {
                ULNNullable = 9999999999,
                LearningDeliveries = new ILR.Tests.Model.TestLearningDelivery[]
                {
                    new ILR.Tests.Model.TestLearningDelivery()
                    {
                        FundModelNullable = 25,
                    },
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 36,
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));
            validationDataServiceMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(false);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_03", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = new ULN_03Rule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);
            rule.Validate(messageLearner);

            validationErrorHandlerMock.Verify(handle, Times.Exactly(2));
        }

        private ULN_03Rule NewRule(IFileDataService fileDataService = null, IValidationDataService validationDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_03Rule(fileDataService, validationDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
