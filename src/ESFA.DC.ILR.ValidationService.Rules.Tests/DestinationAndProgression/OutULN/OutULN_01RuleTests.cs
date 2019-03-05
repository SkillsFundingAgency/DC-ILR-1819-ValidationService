using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutULN
{
    public class OutULN_01RuleTests : AbstractRuleTests<OutULN_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            IDerivedData_15Rule dd15 = new DerivedData_15Rule();
            NewRule(dd15).RuleName.Should().Be("OutULN_01");
        }

        [Theory]
        [InlineData(9999999999)]
        [InlineData(1000000043)]
        public void ValidationPasses(long uln)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            IDerivedData_15Rule dd15 = new DerivedData_15Rule();

            var testLearner = new TestLearnerDestinationAndProgression
            {
                ULN = uln
            };

            NewRule(dd15, validationErrorHandlerMock.Object).Validate(testLearner);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();
            IDerivedData_15Rule dd15 = new DerivedData_15Rule();

            var testLearner = new TestLearnerDestinationAndProgression
            {
                ULN = 21111111111
            };

            NewRule(dd15, validationErrorHandlerMock.Object).Validate(testLearner);
            validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()));
        }

        private OutULN_01Rule NewRule(IDerivedData_15Rule derivedData15Rule, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutULN_01Rule(derivedData15Rule, validationErrorHandler);
        }
    }
}