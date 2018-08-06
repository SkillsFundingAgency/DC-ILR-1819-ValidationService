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
    public class ULN_10RuleTests
    {
        [Fact]
        public void Exclude_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "034")).Returns(true);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Exclude_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, "LDM", "034")).Returns(false);

            var rule = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object);

            rule.Exclude(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(98).Should().BeFalse();
        }

        [Fact]
        public void FAMConditionMet_True()
        {
            NewRule().FAMConditionMet(true).Should().BeTrue();
        }

        [Fact]
        public void FAMConditionMet_False()
        {
            NewRule().FAMConditionMet(false).Should().BeFalse();
        }

        [Fact]
        public void FilePreparationDateMet_True()
        {
            NewRule().FilePreparationDateConditionMet(new DateTime(2030, 1, 1), new DateTime(2018, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void FilePreparationDateMet_False()
        {
            NewRule().FilePreparationDateConditionMet(new DateTime(2010, 1, 1), new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void LearningDatesConditionMet_True_LearnPlanEndDate()
        {
            NewRule().LearningDatesConditionMet(new DateTime(2018, 1, 1), new DateTime(2018, 1, 6), null, new DateTime(2017, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void LearningDatesConditionMet_True_LearnStartDate()
        {
            NewRule().LearningDatesConditionMet(new DateTime(2018, 1, 1), new DateTime(2017, 1, 6), new DateTime(2018, 1, 6), new DateTime(2017, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void LearningDatesConditionMet_False()
        {
            NewRule().LearningDatesConditionMet(new DateTime(2018, 1, 1), new DateTime(2017, 1, 6), new DateTime(2017, 1, 6), new DateTime(2017, 12, 31)).Should().BeFalse();
        }

        [Fact]
        public void LearningDatesConditionMet_FilePreparationDate_False()
        {
            NewRule().LearningDatesConditionMet(new DateTime(2018, 1, 1), new DateTime(2018, 1, 6), new DateTime(2018, 1, 6), new DateTime(2018, 6, 1)).Should().BeFalse();
        }

        [Fact]
        public void UlnConditionMet_True()
        {
            NewRule().UlnConditionMet(9999999999).Should().BeTrue();
        }

        [Fact]
        public void UlnConditionMet_False()
        {
            NewRule().UlnConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(99, true, 9999999999, new DateTime(2018, 1, 1), new DateTime(2018, 1, 1), new DateTime(2018, 1, 2), new DateTime(2017, 1, 7), new DateTime(2018, 1, 7)).Should().BeTrue();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 99,
                        LearnStartDateNullable = new DateTime(2018, 1, 2),
                        LearnPlanEndDateNullable = new DateTime(2017, 1, 7),
                        LearnActEndDateNullable = new DateTime(2018, 1, 7),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[] { }
                    }
                }
            };

            var fileDataMock = new Mock<IFileDataService>();
            var validationDataMock = new Mock<IValidationDataService>();
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(2018, 1, 1));
            validationDataMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "1")).Returns(true);

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("ULN_10", null, null, null);

            validationErrorHandlerMock.Setup(handle);

            var rule = NewRule(fileDataMock.Object, validationDataMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object);

            rule.Validate(learner);

            validationErrorHandlerMock.Verify(handle, Times.Exactly(1));
        }

        [Fact]
        public void Validate_NoErrors_FundModel()
        {
            var learner = new TestLearner()
            {
                ULNNullable = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = 100,
                        LearnStartDateNullable = new DateTime(2018, 1, 2),
                        LearnPlanEndDateNullable = new DateTime(2017, 1, 7),
                        LearnActEndDateNullable = new DateTime(2018, 1, 7),
                    }
                }
            };

            var fileDataMock = new Mock<IFileDataService>();
            var validationDataMock = new Mock<IValidationDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            fileDataMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(2018, 1, 1));
            validationDataMock.SetupGet(vd => vd.AcademicYearJanuaryFirst).Returns(new DateTime(2018, 1, 1));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", "034")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "1")).Returns(false);

            var rule = NewRule(fileDataMock.Object, validationDataMock.Object, learningDeliveryFAMQueryServiceMock.Object);

            rule.Validate(learner);
        }

        private ULN_10Rule NewRule(IFileDataService fileDataService = null, IValidationDataService validationDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_10Rule(fileDataService, validationDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
