using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LLDDCat;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDCat;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDCat
{
    public class LLDDCat_Rule02Tests
    {
        [Theory]
        [InlineData(1, "2015-08-10")]
        [InlineData(2, "2015-08-10")]
        [InlineData(3, "2015-08-10")]
        [InlineData(4, "2100-01-01")]
        [InlineData(17, "2100-01-01")]
        [InlineData(93, "2100-01-01")]
        [InlineData(99, "2100-01-01")]
        public void ConditionMet_True(long? category, string validTo)
        {
            var validToDate = string.IsNullOrEmpty(validTo) ? (DateTime?)null : DateTime.Parse(validTo);

            var lldCatServiceMock = new Mock<ILlddCatInternalDataService>();
            lldCatServiceMock.Setup(x => x.CategoryExistForDate(It.IsAny<long?>(), It.IsAny<DateTime?>())).Returns(false);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(validToDate);

            var rule = NewRule(null, lldCatServiceMock.Object, dd06Mock.Object);
            rule.ConditionMet(category, validToDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, "2015-07-31")]
        [InlineData(2, "2015-07-31")]
        [InlineData(3, "2015-07-31")]
        [InlineData(4, "2099-12-31")]
        [InlineData(17, "2099-12-31")]
        [InlineData(93, "2099-12-31")]
        [InlineData(99, "2099-12-31")]
        [InlineData(99, null)]
        [InlineData(null, null)]
        public void ConditionMet_False(long? category, string validTo)
        {
            var validToDate = string.IsNullOrEmpty(validTo) ? (DateTime?)null : DateTime.Parse(validTo);

            var lldCatServiceMock = new Mock<ILlddCatInternalDataService>();
            lldCatServiceMock.Setup(x => x.CategoryExistForDate(It.IsAny<long?>(), It.IsAny<DateTime?>())).Returns(true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(validToDate);

            var rule = NewRule(null, lldCatServiceMock.Object, dd06Mock.Object);
            rule.ConditionMet(category, validToDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_Null_False()
        {
            var rule = NewRule();
            rule.ConditionMet(null, null).Should().BeFalse();
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    new TestLLDDAndHealthProblem()
                    {
                        LLDDCatNullable = 1
                    }
                }
            };

            var lldCatServiceMock = new Mock<ILlddCatInternalDataService>();
            lldCatServiceMock.Setup(x => x.CategoryExistForDate(It.IsAny<long?>(), It.IsAny<DateTime?>())).Returns(true);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2010, 10, 10));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDCat_02", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object, lldCatServiceMock.Object, dd06Mock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    new TestLLDDAndHealthProblem()
                    {
                        LLDDCatNullable = 1
                    }
                }
            };

            var lldCatServiceMock = new Mock<ILlddCatInternalDataService>();
            lldCatServiceMock.Setup(x => x.CategoryExistForDate(It.IsAny<long?>(), It.IsAny<DateTime?>())).Returns(false);

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2010, 10, 10));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("LLDDCat_02", null, null, null);

            var rule = NewRule(validationErrorHandlerMock.Object, lldCatServiceMock.Object, dd06Mock.Object);
            rule.Validate(learner);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        private LLDDCat_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILlddCatInternalDataService llddCatDataService = null, IDD06 dd06 = null)
        {
            return new LLDDCat_02Rule(validationErrorHandler, llddCatDataService, dd06);
        }
    }
}