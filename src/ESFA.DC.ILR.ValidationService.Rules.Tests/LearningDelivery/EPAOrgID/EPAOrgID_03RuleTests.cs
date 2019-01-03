using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_03RuleTests : AbstractRuleTests<EPAOrgID_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EPAOrgID_03");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var appFinCodes = new[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice, appFinCodes))
                .Returns(false);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet("X", It.IsAny<IEnumerable<IAppFinRecord>>())
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ConditionMet_False_EPAOrgID(string epaOrgID)
        {
            NewRule().ConditionMet(epaOrgID, It.IsAny<IEnumerable<IAppFinRecord>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AppFinRecords()
        {
            var appFinCodes = new[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(It.IsAny<IEnumerable<IAppFinRecord>>(), ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice, appFinCodes))
                .Returns(true);

            NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object).ConditionMet("X", It.IsAny<IEnumerable<IAppFinRecord>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = "X",
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 1,
                                AFinType = "XXX"
                            }
                        }
                    }
                }
            };

            var appFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var appFinCodes = new[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice, appFinCodes))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = "X",
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 2,
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            var appFinRecords = learner.LearningDeliveries.SelectMany(ld => ld.AppFinRecords);

            var appFinCodes = new[] { 2, 4 };

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock
                .Setup(qs => qs.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice, appFinCodes))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryAppFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("EPAOrgID", "X")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("X");

            validationErrorHandlerMock.Verify();
        }

        private EPAOrgID_03Rule NewRule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EPAOrgID_03Rule(learningDeliveryAppFinRecordQueryService, validationErrorHandler);
        }
    }
}
