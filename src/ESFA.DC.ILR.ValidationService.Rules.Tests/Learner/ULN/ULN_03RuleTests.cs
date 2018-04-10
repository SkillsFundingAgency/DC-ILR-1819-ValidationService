using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
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
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learner = new TestLearner();

            var learnerQueryServiceMock = new Mock<ILearnerQueryService>();

            learnerQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner, "ACT", "1")).Returns(true);

            NewRule(learnerQueryService: learnerQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learner).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learner = new TestLearner();

            var learnerQueryServiceMock = new Mock<ILearnerQueryService>();

            learnerQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner, "ACT", "1")).Returns(false);

            NewRule(learnerQueryService: learnerQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learner).Should().BeTrue();
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
            var learner = new TestLearner()
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

            var fileDataServiceMock = new Mock<IFileDataCache>();
            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.JanuaryFirst()).Returns(new DateTime(2018, 1, 1));

            var learnerQueryServiceMock = new Mock<ILearnerQueryService>();
            learnerQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner, "ACT", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(fileDataServiceMock.Object, academicYearDataServiceMock.Object, learnerQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new ILR.Tests.Model.TestLearner()
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

            var fileDataServiceMock = new Mock<IFileDataCache>();
            fileDataServiceMock.SetupGet(fd => fd.FilePreparationDate).Returns(new DateTime(1970, 1, 1));

            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();
            academicYearDataServiceMock.Setup(ds => ds.JanuaryFirst()).Returns(new DateTime(2018, 1, 1));

            var learnerQueryServiceMock = new Mock<ILearnerQueryService>();
            learnerQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner, "ACT", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError("ULN_03"))
            {
                NewRule(fileDataServiceMock.Object, academicYearDataServiceMock.Object, learnerQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private ULN_03Rule NewRule(IFileDataCache fileDataCache = null, IAcademicYearDataService academicYearDataService = null, ILearnerQueryService learnerQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_03Rule(fileDataCache, academicYearDataService, learnerQueryService, validationErrorHandler);
        }
    }
}
