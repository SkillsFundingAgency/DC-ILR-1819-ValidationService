using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EPAOrgID;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.EPAOrgID
{
    public class EPAOrgID_02RuleTests : AbstractRuleTests<EPAOrgID_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EPAOrgID_02");
        }

        [Theory]
        [InlineData("Epa001", "TNP", 2)]
        [InlineData("Epa001", "TNP", 4)]
        public void ConditionMet_False(string epaOrgId, string aFinType, int aFinCode)
        {
            NewRule().ConditionMet(epaOrgId, aFinType, aFinCode).Should().BeFalse();
        }

        [Theory]
        [InlineData("", "TNP", 2)]
        [InlineData("", "TNP", 4)]
        public void ConditionMet_True(string epaOrgId, string aFinType, int aFinCode)
        {
            NewRule().ConditionMet(epaOrgId, aFinType, aFinCode).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "TNP", 2)]
        [InlineData("", "TNP", 4)]
        public void Validate_Error(string epaOrgId, string aFinType, int aFinCode)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = epaOrgId,
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = aFinCode,
                                AFinType = aFinType
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
            }
        }

        [Fact]
        public void Validate_Error_Multiple()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 2,
                                AFinType = "TNP"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 4,
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        EPAOrgID = "EPA001",
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 2,
                                AFinType = "TNP"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        EPAOrgID = "EPA002",
                        AppFinRecords = new List<TestAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinCode = 4,
                                AFinType = "TNP"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int aFinCode = 2;
            string aFinType = "TNP";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinType", "TNP")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AFinCode", 2)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aFinType, aFinCode);

            validationErrorHandlerMock.Verify();
        }

        private EPAOrgID_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new EPAOrgID_02Rule(validationErrorHandler);
        }
    }
}