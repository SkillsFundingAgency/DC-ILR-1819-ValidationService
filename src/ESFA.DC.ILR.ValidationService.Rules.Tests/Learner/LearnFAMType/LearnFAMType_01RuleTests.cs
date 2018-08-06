using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_01RuleTests
    {
        [Theory]
        [InlineData("XYZ", 1)]
        [InlineData("LSR", 99)]
        public void ConditionMet_True(string famType, long famCode)
        {
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService(famType, famCode, false);
            var rule = NewRule(null, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.ConditionMet(famType, famCode).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 1)]
        [InlineData("LSR", 1)]
        [InlineData("LSR", null)]
        public void ConditionMet_False(string famType, long famCode)
        {
            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService(famType, famCode, true);
            var rule = NewRule(null, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.ConditionMet(famType, famCode).Should().BeFalse();
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

            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LearnFAMType_01", null, null, null);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService(It.IsAny<string>(), It.IsAny<long>(), false);

            var rule = NewRule(validationErrorHandlerMock.Object, learnFamTypeCodeInternalDataServiceMock.Object);
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

            var learnFamTypeCodeInternalDataServiceMock = SetupLearnFamTypeMockService("MCF", 1, true);

            var rule = NewRule(validationErrorHandlerMock.Object, learnFamTypeCodeInternalDataServiceMock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private LearnFAMType_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnFAMTypeCodeInternalDataService ilLearnFamTypeCodeInternalDataService = null)
        {
            return new LearnFAMType_01Rule(validationErrorHandler, ilLearnFamTypeCodeInternalDataService);
        }

        private Mock<ILearnFAMTypeCodeInternalDataService> SetupLearnFamTypeMockService(string famType, long famCode, bool result)
        {
            var learnFamTypeCodeInternalDataServiceMock = new Mock<ILearnFAMTypeCodeInternalDataService>();
            learnFamTypeCodeInternalDataServiceMock.Setup(x => x.TypeCodeExists(famType, famCode)).Returns(result);
            return learnFamTypeCodeInternalDataServiceMock;
        }
    }
}