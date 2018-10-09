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

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void TNPConditionMet_True(int? aFinCode)
        {
            NewRule().TNPConditionMet(aFinCode).Should().BeTrue();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(null)]
        public void TNPConditionMet_False(int? aFinCode)
        {
            NewRule().TNPConditionMet(aFinCode).Should().BeFalse();
        }

        [Fact]
        public void PMRConditionMet_True()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).PMRConditionMet(aimType, appFinRecords).Should().BeTrue();
        }

        [Fact]
        public void PMRConditionMet_False()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).PMRConditionMet(aimType, appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords, afinCode).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_PMR()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords, afinCode).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_TNP()
        {
            var aimType = 1;
            var afinCode = 2;
            var aFinType = "PMR";

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).ConditionMet(aimType, appFinRecords, afinCode).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

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
                                AFinCode = 1,
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var aimType = 1;
            var afinCode = 1;
            var aFinType = "PMR";

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
                                AFinCode = 1,
                                AFinType = "TNP"
                            },
                            new TestAppFinRecord
                            {
                                AFinCode = 1,
                                AFinType = "PMR"
                            }
                        }
                    }
                }
            };

            var appFinRecords = new List<IAppFinRecord>();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();

            appFinRecordQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryAFinCodeForType(It.IsAny<IEnumerable<IAppFinRecord>>(), aFinType, afinCode)).Returns(true);

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
