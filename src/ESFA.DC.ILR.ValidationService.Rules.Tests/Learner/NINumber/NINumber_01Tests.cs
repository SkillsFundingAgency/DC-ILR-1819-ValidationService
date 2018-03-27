using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.NINumber
{
    public class NINumber_01Tests
    {
        [Theory]
        [InlineData("DZ0123456C")]
        [InlineData("FZ0123456C")]
        [InlineData("IZ0123456C")]
        [InlineData("QZ0123456C")]
        [InlineData("UZ0123456C")]
        [InlineData("VZ0123456C")]
        public void ConditionMet_True_FirstCharacter(string niNumber)
        {
            var rule = new NINumber_01Rule(null);

            rule.ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData("AD0123456C")]
        [InlineData("AF0123456C")]
        [InlineData("AI0123456C")]
        [InlineData("AO0123456C")]
        [InlineData("AQ0123456C")]
        [InlineData("AU0123456C")]
        [InlineData("AV0123456C")]
        public void ConditionMet_True_SecondCharacter(string niNumber)
        {
            var rule = new NINumber_01Rule(null);

            rule.ConditionMet(niNumber).Should().BeTrue();
        }

        [Theory]
        [InlineData("AXA23456C")]
        [InlineData("AX1X3456C")]
        [InlineData("AX12X456C")]
        [InlineData("AX123X56C")]
        [InlineData("AX1234X6C")]
        [InlineData("AX12345XC")]
        public void ConditionMet_True_ThreeTo8Characters(string niNumber)
        {
            var rule = new NINumber_01Rule(null);

            rule.ConditionMet(niNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_LastCharacter()
        {
            var rule = new NINumber_01Rule(null);

            for (char letter = 'E'; letter <= 'Z'; letter++)
            {
                var niNumber = $"AX123456{letter}";
                rule.ConditionMet(niNumber).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("AX123456c")]
        [InlineData("aX123456D")]
        [InlineData("Ab123456A")]
        public void ConditionMet_True_LowerCase(string niNumber)
        {
            var rule = new NINumber_01Rule(null);
            rule.ConditionMet(niNumber).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFirstLetter()
        {
            var notAllowed = new List<char>() { 'D', 'F', 'I', 'Q', 'U', 'V' };
            char[] eToZ = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            var rule = new NINumber_01Rule(null);

            foreach (char letter in eToZ)
            {
                var niNumber = $"{letter}X123456C";
                rule.ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_FalseSecondLetter()
        {
            var notAllowed = new List<char>() { 'D', 'F', 'I', 'O', 'Q', 'U', 'V' };
            char[] eToZ = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).Where(x => !notAllowed.Contains(x)).ToArray();

            var rule = new NINumber_01Rule(null);

            foreach (char letter in eToZ)
            {
                var niNumber = $"A{letter}123456B";
                rule.ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_FalseLastLetter()
        {
            var allowed = new List<char>() { 'A', 'B', 'C', 'D', ' ' };
            var rule = new NINumber_01Rule(null);

            foreach (char letter in allowed)
            {
                var niNumber = $"AX123456{letter}";
                rule.ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData("AX{0}23456C")]
        [InlineData("AX1{0}3456C")]
        [InlineData("AX12{0}456C")]
        [InlineData("AX123{0}56C")]
        [InlineData("AX2234{0}6C")]
        [InlineData("AX12345{0}C")]
        public void ConditionMet_FalseNumbers(string niTemplate)
        {
            var rule = new NINumber_01Rule(null);

            for (var i = 0; i < 10; i++)
            {
                var niNumber = string.Format(niTemplate, i);
                rule.ConditionMet(niNumber).Should().BeFalse();
            }
        }

        [Fact]
        public void ConditionMet_FalseEmptyString()
        {
            var rule = new NINumber_01Rule(null);

            rule.ConditionMet("  ").Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseEmptyNull()
        {
            var rule = new NINumber_01Rule(null);

            rule.ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.NINumber).Returns("AZ123456C");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("NINumber_01", null, null, null);

            var rule = new NINumber_01Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner.Object);
            validationErrorHandlerMock.Verify(handle, Times.Never);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.NINumber).Returns("AO123456X");

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            Expression<Action<IValidationErrorHandler>> handle = veh => veh.Handle("NINumber_01", null, null, null);

            var rule = new NINumber_01Rule(validationErrorHandlerMock.Object);

            rule.Validate(learner.Object);
            validationErrorHandlerMock.Verify(handle, Times.Once);
        }
    }
}
