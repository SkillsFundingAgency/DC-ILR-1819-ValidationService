using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_02RuleTests : AbstractRuleTests<EngGrade_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EngGrade_02");
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            string engGrade = "33310";

            var validaionErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validaionErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.EngGrade, engGrade)).Verifiable();

            NewRule(validationErrorHandler: validaionErrorHandlerMock.Object).BuildErrorMessageParameters(engGrade);

            validaionErrorHandlerMock.Verify();
        }

        [Fact]
        public void ConditionMet_False()
        {
            string learnAimRef = "ABC123456";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).ConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).ConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                EngGrade = "33310",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Theory]
        [InlineData("123")]
        [InlineData("A**")]
        public void Validate_EngGrade_AimTypeGCSE_NoError(string grade)
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                EngGrade = grade,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Theory]
        [InlineData("123")]
        [InlineData("A**")]
        public void Validate_EngGrade_AimTypeNotGCSE_Error(string grade)
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                EngGrade = grade,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999", "NONE" };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoGradeSupplied_NoError()
        {
            string learnAimRef = "ABC98765";
            HashSet<string> learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999" };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "AB12345",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, learnAimRefTypes)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        public EngGrade_02Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService lARSDataService = null)
        {
            return new EngGrade_02Rule(
                validationErrorHandler: validationErrorHandler,
                lARSDataService: lARSDataService);
        }
    }
}
