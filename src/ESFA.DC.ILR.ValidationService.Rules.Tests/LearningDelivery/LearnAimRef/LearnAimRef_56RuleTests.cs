using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_56RuleTests : AbstractRuleTests<LearnAimRef_56Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_56");
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery { LearnAimRef = "123456" };

            Mock<ILARSDataService> larsDataService = new Mock<ILARSDataService>();

            larsDataService.Setup(qs => qs.LearnAimRefExistsForLearningDeliveryCategoryRef("123456", 22)).Returns(true);

            NewRule(larsDataService.Object).LARSConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery { LearnAimRef = "123456" };

            Mock<ILARSDataService> larsDataService = new Mock<ILARSDataService>();

            larsDataService.Setup(qs => qs.LearnAimRefExistsForLearningDeliveryCategoryRef("12345689", 22)).Returns(false);

            NewRule(larsDataService.Object).LARSConditionMet(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, 2018, 08, 01, "LDM", "034", true)]
        [InlineData(70, 2018, 08, 01, "LDM", "034", true)]
        [InlineData(81, 2018, 08, 01, "LDM", "034", true)]
        [InlineData(81, 2018, 08, 01, "LDM", "328", true)]
        public void ConditionMet_True(int fundModel, int yyyy, int mm, int dd, string famType, string famCode, bool mockBool)
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "123456",
                FundModel = fundModel,
                LearnStartDate = new DateTime(yyyy, mm, dd),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = famType,
                        LearnDelFAMCode = famCode
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", new List<string> { "034", "328" })).Returns(mockBool);

            NewRule(null, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, 2018, 08, 01, "LDM", "034", true)]
        [InlineData(70, 2014, 08, 01, "LDM", "034", true)]
        [InlineData(81, 2018, 08, 01, "LDM", "040", false)]
        public void ConditionMet_False(int fundModel, int yyyy, int mm, int dd, string famType, string famCode, bool mockBool)
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = "123456",
                FundModel = fundModel,
                LearnStartDate = new DateTime(yyyy, mm, dd),
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = famType,
                        LearnDelFAMCode = famCode
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", new List<string> { "034", "328" })).Returns(mockBool);

            NewRule(null, learningDeliveryFAMQueryServiceMock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "123456",
                        FundModel = 35,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "034"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataService = new Mock<ILARSDataService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", new List<string> { "034", "328" })).Returns(true);
            larsDataService.Setup(qs => qs.LearnAimRefExistsForLearningDeliveryCategoryRef("123456", 22)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataService.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_LARSConditionMet()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "123456",
                        FundModel = 35,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "034"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataService = new Mock<ILARSDataService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", new List<string> { "034", "328" })).Returns(true);
            larsDataService.Setup(qs => qs.LearnAimRefExistsForLearningDeliveryCategoryRef("123456", 22)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataService.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ConditionMet()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearnAimRef = "123456",
                        FundModel = 35,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "034"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataService = new Mock<ILARSDataService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasAnyLearningDeliveryFAMCodesForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", new List<string> { "034", "328" })).Returns(false);
            larsDataService.Setup(qs => qs.LearnAimRefExistsForLearningDeliveryCategoryRef("123456", 22)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataService.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "123456")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 35)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("123456", new DateTime(2018, 08, 01), 35);

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_56Rule NewRule(ILARSDataService larsDataService = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_56Rule(larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
