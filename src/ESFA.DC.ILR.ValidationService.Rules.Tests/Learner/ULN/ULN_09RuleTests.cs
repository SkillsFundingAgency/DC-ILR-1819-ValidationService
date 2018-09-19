using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
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
    public class ULN_09RuleTests : AbstractRuleTests<ULN_09Rule>
    {
        [Fact]
        public void ULNConditionMet_True()
        {
            NewRule().ULNConditionMet(9999999999).Should().BeTrue();
        }

        [Fact]
        public void ULNConditionMet_False()
        {
            NewRule().ULNConditionMet(1111111111).Should().BeFalse();
        }

        [Fact]
        public void FilePrepDateConditionMet_True()
        {
            var filePrepDate = new DateTime(2019, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            NewRule().FilePrepDateConditionMet(filePrepDate, januraryFirst).Should().BeTrue();
        }

        [Fact]
        public void FilePrepDateConditionMet_False()
        {
            var filePrepDate = new DateTime(2018, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            NewRule().FilePrepDateConditionMet(filePrepDate, januraryFirst).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_FAMType()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False_NullFAMS()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var uln = 9999999999;
            var filePrepDate = new DateTime(2019, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(uln, filePrepDate, januraryFirst, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ULN()
        {
            var uln = 1111111111;
            var filePrepDate = new DateTime(2019, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(uln, filePrepDate, januraryFirst, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FilePrepDate()
        {
            var uln = 9999999999;
            var filePrepDate = new DateTime(2018, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(uln, filePrepDate, januraryFirst, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FAMType()
        {
            var uln = 9999999999;
            var filePrepDate = new DateTime(2019, 1, 1);
            var januraryFirst = new DateTime(2019, 1, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "LDM", "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(uln, filePrepDate, januraryFirst, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "034"
                }
            };
            var learner = new TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var academicDataQueryServiceMock = new Mock<IAcademicYearDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicDataQueryServiceMock.Setup(qs => qs.JanuaryFirst()).Returns(new DateTime(2019, 01, 01));
            fileDataServiceMock.Setup(fd => fd.FilePreparationDate()).Returns(new DateTime(2019, 01, 01));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicDataQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "034"
                }
            };
            var learner = new TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var academicDataQueryServiceMock = new Mock<IAcademicYearDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            academicDataQueryServiceMock.Setup(qs => qs.JanuaryFirst()).Returns(new DateTime(2019, 01, 01));
            fileDataServiceMock.Setup(fd => fd.FilePreparationDate()).Returns(new DateTime(2019, 01, 01));
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    academicDataQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", (long)1234567890)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FilePrepDate", "01/01/2019")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1234567890, new DateTime(2019, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private ULN_09Rule NewRule(
            IFileDataService fileDataService = null,
            IAcademicYearDataService academicDataQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_09Rule(fileDataService, academicDataQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
