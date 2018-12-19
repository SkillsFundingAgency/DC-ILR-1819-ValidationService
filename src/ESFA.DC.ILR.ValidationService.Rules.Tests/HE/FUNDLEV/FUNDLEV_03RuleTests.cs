using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FUNDLEV
{
    public class FUNDLEV_03RuleTests : AbstractRuleTests<FUNDLEV_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FUNDLEV_03");
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2009, 08, 02)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2009, 07, 31)).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(It.IsAny<string>()).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(It.IsAny<string>()).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHeConditionMet_True()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = 0
            };

            NewRule().LearningDeliveryHeConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHeConditionMet_False_Null()
        {
            NewRule().LearningDeliveryHeConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(99)]
        public void LearningDeliveryHeConditionMet_False(int fundLev)
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = fundLev
            };

            NewRule().LearningDeliveryHeConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2009, 08, 02);
            var learnAimRef = "123456789";

            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = 0
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnStartDate, learnAimRef, learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var learnStartDate = new DateTime(2009, 07, 31);
            var learnAimRef = "123456789";

            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = 0
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnStartDate, learnAimRef, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LARS()
        {
            var learnStartDate = new DateTime(2009, 08, 02);
            var learnAimRef = "123456789";

            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = 0
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<IEnumerable<string>>()))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnStartDate, learnAimRef, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningDeliveryHe()
        {
            var learnStartDate = new DateTime(2009, 08, 02);
            var learnAimRef = "123456789";

            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                FUNDLEV = 20
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).ConditionMet(learnStartDate, learnAimRef, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2009, 08, 02),
                        LearnAimRef = "123456789",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDLEV = 0
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2009, 08, 02),
                        LearnAimRef = "123456789",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            FUNDLEV = 20
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FUNDLEV", 0)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 0);

            validationErrorHandlerMock.Verify();
        }

        private FUNDLEV_03Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new FUNDLEV_03Rule(larsDataService, validationErrorHandler);
        }
    }
}
