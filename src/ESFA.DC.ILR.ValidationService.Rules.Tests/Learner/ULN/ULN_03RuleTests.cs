using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_03RuleTests : AbstractRuleTests
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

            NewRule(learningDeliveryFAMQueryService : learningDeliveryFAMQueryServiceMock.Object).Exclude(learningDelivery).Should().BeFalse();
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
            NewRule().ConditionMet(fundModel, 9999999999, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Uln()
        {
            NewRule().ConditionMet(25, 1, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            NewRule().ConditionMet(1, 9999999999, new DateTime(1970, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FilePreparationDate()
        {
            NewRule().ConditionMet(25, 9999999999, new DateTime(2030, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            var messageLearner = new TestLearner()
            {
                ULN = 1,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 2,
                    }
                }
            };

            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));
            validationDataServiceMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(messageLearner);
            }
        }

        [Fact]
        public void Validate_Errors()
        {
            var messageLearner = new ILR.Tests.Model.TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            var validationDataServiceMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));
            validationDataServiceMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "ACT", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError("ULN_03"))
            {
                NewRule(fileDataServiceMock.Object, validationDataServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(messageLearner);
            }
        }

        private ULN_03Rule NewRule(IFileDataService fileDataService = null, IValidationDataService validationDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_03Rule(fileDataService, validationDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
