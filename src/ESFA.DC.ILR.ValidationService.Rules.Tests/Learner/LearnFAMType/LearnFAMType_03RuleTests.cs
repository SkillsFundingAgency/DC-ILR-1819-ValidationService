using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_03RuleTests
    {
        [Fact]
        public void ConditionMet_True()
        {
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService("ECF", 1, new DateTime(2100, 01, 01), false);
            var rule = NewRule(null, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.ConditionMet("ECF", 1, new DateTime(2100, 01, 01)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService("ECF", 1, new DateTime(2099, 12, 31), true);
            var rule = NewRule(null, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.ConditionMet("ECF", 1, new DateTime(2099, 12, 31)).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 1, "2099-12-31")]
        [InlineData("ECF", null, "2099-12-31")]
        [InlineData(null, null, "2099-12-31")]
        [InlineData("ECF", 1, null)]
        [InlineData(null, null, null)]
        public void ConditionMet_False_Null(string famType, long? famCode, string validTo)
        {
            var validToDate = string.IsNullOrEmpty(validTo) ? (DateTime?)null : DateTime.Parse(validTo);

            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService(famType, famCode, validToDate, true);
            var rule = NewRule(null, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.ConditionMet(famType, famCode, validToDate).Should().BeFalse();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ",
                        LearnFAMCodeNullable = 1
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_03", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var dd06Mock = SetupDD06MockService(new DateTime(2100, 01, 01));
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<DateTime>(), false);

            var rule = NewRule(validationErrorHandlerMock.Object, learnFamTypeCodeInternalDataServiceMock.Object, dd06Mock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "MCF",
                        LearnFAMCodeNullable = 1
                    }
                }
            };

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var dd06Mock = SetupDD06MockService(new DateTime(2099, 12, 31));
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService("MCF", 1, new DateTime(2099, 12, 31), true);

            var rule = NewRule(validationErrorHandlerMock.Object, learnFamTypeCodeInternalDataServiceMock.Object, dd06Mock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Mock<ILearnFAMTypeCodeInternalDataService> SetupLearnFamTypeMockService(string famType, long? famCode, DateTime? validTo, bool result)
        {
            var learnFamTypeCodeInternalDataServiceMock = new Mock<ILearnFAMTypeCodeInternalDataService>();
            learnFamTypeCodeInternalDataServiceMock.Setup(x => x.TypeCodeForDateExists(famType, famCode, validTo)).Returns(result);
            return learnFamTypeCodeInternalDataServiceMock;
        }

        private LearnFAMType_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnFAMTypeCodeInternalDataService ilLearnFamTypeCodeInternalDataService = null, IDD06 dd06 = null)
        {
            return new LearnFAMType_03Rule(validationErrorHandler, ilLearnFamTypeCodeInternalDataService, dd06);
        }

        private Mock<IDD06> SetupDD06MockService(DateTime? result)
        {
            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x => x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(result);
            return dd06Mock;
        }
    }
}