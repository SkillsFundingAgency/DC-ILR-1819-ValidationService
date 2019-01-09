using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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
    public class AFinType_14RuleTests : AbstractRuleTests<AFinType_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinType_14");
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(3).Should().BeFalse();
        }

        [Fact]
        public void PMRConditionMet_True()
        {
            var afinCode = 1;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).PMRConditionMet(appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void PMRConditionMet_False()
        {
            var afinCode = 2;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).PMRConditionMet(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void TNPConditionMet_True()
        {
            var aFinType = "TNP";
            var aFinCodes = new int[] { 1, 3 };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, aFinCodes)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).TNPConditionMet(appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void TNPConditionMet_False()
        {
            var aFinType = "TNP";
            var aFinCodes = new int[] { 1, 3 };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, aFinCodes)).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).TNPConditionMet(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(true);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_PMR()
        {
            var aimType = 1;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(false);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_TNP()
        {
            var aimType = 1;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(true);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var aimType = 2;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(false);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 1;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinCode = afinCodePMR,
                                AFinType = aFinTypePMR
                            }
                        }
                    }
                }
            };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(true);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var aimType = 1;
            var afinCodePMR = 1;
            var aFinTypePMR = "PMR";
            var aFinCodesTNP = new int[] { 1, 3 };
            var aFinTypeTNP = "TNP";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinCode = afinCodePMR,
                                AFinType = aFinTypePMR
                            }
                        }
                    }
                }
            };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypePMR, afinCodePMR)).Returns(true);
            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinTypeTNP, aFinCodesTNP)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinType", "TNP")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinCode", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("TNP", 1);

            validationErrorHandlerMock.Verify();
        }

        private AFinType_14Rule NewRule(ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinType_14Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
