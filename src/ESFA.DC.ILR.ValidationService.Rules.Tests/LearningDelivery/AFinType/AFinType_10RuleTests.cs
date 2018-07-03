using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinType
{
    public class AFinType_10RuleTests : AbstractRuleTests<AFinType_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_10");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardConditionMet_True()
        {
            NewRule().ApprenticeshipStandardConditionMet(25, 1).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipStandardConditionMet_False()
        {
            NewRule().ApprenticeshipStandardConditionMet(20, 1).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardConditionMet_False_NullProgType()
        {
            NewRule().ApprenticeshipStandardConditionMet(null, 1).Should().BeFalse();
        }

        [Fact]
        public void AppFinRecordConditionMet_True()
        {
            var aFinCodes = new List<int> { 2, 4 };

            var aFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1
                }
            };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(aFinRecords).Should().BeTrue();
        }

        [Theory]
        [InlineData("TNP", 2)]
        [InlineData("PMR", 2)]
        [InlineData("PMR", 3)]
        public void AppFinRecordConditionMet_False(string aFinType, int aFinCode)
        {
            var aFinCodes = new List<int> { 2, 4 };

            var aFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).AppFinRecordConditionMet(aFinRecords).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, 25, 1, "TNP", 1)]
        [InlineData(35, 25, 1, "TNP", 3)]
        [InlineData(35, 25, 1, "PMR", 2)]
        public void ConditionMet_True(int fundModel, int? progType, int aimType, string aFinType, int aFinCode)
        {
            var aFinCodes = new List<int> { 2, 4 };

            var aFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, progType, aimType, aFinRecords).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, 25, 1, "TNP", 2)]
        [InlineData(35, 20, 1, "TNP", 2)]
        [InlineData(35, 25, 2, "TNP", 2)]
        [InlineData(35, 25, 1, "TNP", 2)]
        public void ConditionMet_False(int fundModel, int? progType, int aimType, string aFinType, int aFinCode)
        {
            var aFinCodes = new List<int> { 2, 4 };

            var aFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = aFinType,
                    AFinCode = aFinCode
                }
            };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet(fundModel, progType, aimType, aFinRecords).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aFinCodes = new List<int> { 2, 4 };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 1
                            }
                        }
                    }
                }
            };

            var aFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var aFinCodes = new List<int> { 2, 4 };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        AimType = 1,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = "TNP",
                                AFinCode = 2
                            }
                        }
                    }
                }
            };

            var aFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            learningDeliveryAppFinRecordQueryServiceMock.Setup(afm => afm.HasAnyLearningDeliveryAFinCodesForType(aFinRecords, "TNP", aFinCodes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinType_10Rule NewRule(ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_10Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
