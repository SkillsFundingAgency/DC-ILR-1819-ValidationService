using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutType
{
    public class OutType_02RuleTests : AbstractRuleTests<OutType_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutType_02");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outTypes = new List<string>
            {
                "GAP",
                "GAP",
                "EDU"
            };

            NewRule().ConditionMet(outTypes).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_Multiple()
        {
            var outTypes = new List<string>
            {
                "GAP",
                "GAP",
                "EDU",
                "EDU",
                "EMP",
                "EMP"
            };

            NewRule().ConditionMet(outTypes).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var outTypes = new List<string>
            {
                "EDU",
                "GAP"
            };

            NewRule().ConditionMet(outTypes).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Multiple()
        {
            var outTypes = new List<string>
            {
                "EDU",
                "EDU",
                "GAP",
            };

            NewRule().ConditionMet(outTypes).Should().BeFalse();
        }

        [Theory]
        [InlineData("GAP", "GAP", "EDU", "EMP")]
        [InlineData("GAP", "GAP", "EDU", "EDU")]
        [InlineData("GAP", "GAP", "EMP", "EMP")]
        public void Validate_Errors(string outType1, string outType2, string outType3, string outType4)
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
            };

            IDictionary<DateTime, IEnumerable<string>> outTypeDictionary = new Dictionary<DateTime, IEnumerable<string>>
            {
                { new DateTime(2018, 8, 1), new List<string> { outType1, outType2 } },
                { new DateTime(2018, 9, 1), new List<string> { outType1, outType2, outType3, outType4 } }
            };

            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock.Setup(qs => qs.OutTypesForStartDateAndTypes(learnerDP.DPOutcomes, It.IsAny<IEnumerable<string>>())).Returns(outTypeDictionary);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Theory]
        [InlineData("GAP", "EDU", "EMP")]
        [InlineData("GAP", "EDU", "EDU")]
        [InlineData("GAP", "EMP", "OTH")]
        public void Validate_NoErrors(string outType1, string outType2, string outType3)
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>()
            };

            IDictionary<DateTime, IEnumerable<string>> outTypeDictionary = new Dictionary<DateTime, IEnumerable<string>>
            {
                { new DateTime(2018, 8, 1), new List<string> { outType1, outType2 } },
                { new DateTime(2018, 9, 1), new List<string> { outType1, outType2, outType3 } }
            };

            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock.Setup(qs => qs.OutTypesForStartDateAndTypes(learnerDP.DPOutcomes, It.IsAny<IEnumerable<string>>())).Returns(outTypeDictionary);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerDPQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutStartDate", "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutType", "GAP")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1), "GAP");

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void BuildErrorMessageParameters_Multiple()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutStartDate", "01/01/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutType", "GAP, EMP")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 1, 1), "GAP, EMP");

            validationErrorHandlerMock.Verify();
        }

        private OutType_02Rule NewRule(ILearnerDPQueryService learnerDPQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutType_02Rule(learnerDPQueryService, validationErrorHandler);
        }
    }
}
