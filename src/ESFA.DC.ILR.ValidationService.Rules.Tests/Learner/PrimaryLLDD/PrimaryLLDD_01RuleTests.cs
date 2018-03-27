using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_01RuleTests
    {
        [Fact]
        public void ConditionMet_True_NullLlddCategories()
        {
            var rule = NewRule();
            rule.ConditionMet(null, new DateTime(2015, 09, 01)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(99),
                SetupLlddHealthAndProblem(null),
            };
            rule.ConditionMet(llDDAndHealthProblems, new DateTime(2015, 09, 01)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullLearningStartDate()
        {
            var rule = NewRule(null, null);
            rule.ConditionMet(It.IsAny<IReadOnlyCollection<ILLDDAndHealthProblem>>(), null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearningStartDate()
        {
            var rule = NewRule(null, null);
            rule.ConditionMet(null, new DateTime(2015, 07, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(1),
                SetupLlddHealthAndProblem(100),
            };
            rule.ConditionMet(llDDAndHealthProblems, new DateTime(2015, 08, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMetAnyValidPrimaryLldd_True_NullValue()
        {
            var rule = NewRule(null, null);
            rule.ConditionMetAnyValidPrimaryLldd(null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetAnyValidPrimaryLldd_True_InvalidValue()
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(99),
                SetupLlddHealthAndProblem(100)
            };
            rule.ConditionMetAnyValidPrimaryLldd(llDDAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void ConditionMetAnyValidPrimaryLldd_False()
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(1),
                SetupLlddHealthAndProblem(100)
            };
            rule.ConditionMetAnyValidPrimaryLldd(llDDAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void Exclude_False()
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(null, 9999),
                SetupLlddHealthAndProblem(null, 100),
                SetupLlddHealthAndProblem(null, null)
            };
            rule.Exclude(llDDAndHealthProblems).Should().BeFalse();
        }

        [Theory]
        [InlineData(99)]
        [InlineData(98)]
        public void Exclude_True(long? excludeCatValue)
        {
            var rule = NewRule(null, null);

            var llDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                SetupLlddHealthAndProblem(null, excludeCatValue),
                SetupLlddHealthAndProblem(200, excludeCatValue),
            };
            rule.Exclude(llDDAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void Validate_False()
        {
            var validationErrorHandlerMock = SetupForValidate(9999);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_01", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }

        [Fact]
        public void Validate_True()
        {
            var validationErrorHandlerMock = SetupForValidate(1);
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("PrimaryLLDD_01", null, null, null);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        private Mock<IValidationErrorHandler> SetupForValidate(long? primaryLldValue)
        {
            var learner = new TestLearner()
            {
                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    SetupLlddHealthAndProblem(primaryLldValue, 10),
                    SetupLlddHealthAndProblem(null, 100),
                    SetupLlddHealthAndProblem(null, null)
                }
            };

            var dd06Mock = new Mock<IDD06>();
            dd06Mock.Setup(x =>
                x.Derive(It.IsAny<IReadOnlyCollection<ILearningDelivery>>())).Returns(new DateTime(2015, 08, 31));

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var rule = NewRule(validationErrorHandlerMock.Object, dd06Mock.Object);
            rule.Validate(learner);
            return validationErrorHandlerMock;
        }

        private PrimaryLLDD_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IDD06 dd06 = null)
        {
            return new PrimaryLLDD_01Rule(validationErrorHandler, dd06);
        }

        private TestLLDDAndHealthProblem SetupLlddHealthAndProblem(long? primarylldValue, long? lldCat = 1)
        {
            return new TestLLDDAndHealthProblem()
            {
                LLDDCatNullable = lldCat,
                PrimaryLLDDNullable = primarylldValue
            };
        }
    }
}