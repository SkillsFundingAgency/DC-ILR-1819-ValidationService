using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.DestinationAndProgression.OutType
{
    public class OutType_01RuleTests : AbstractRuleTests<OutType_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OutType_01");
        }

        [Fact]
        public void OutTypeConditionMet_True()
        {
            var dpOutcome = new TestDPOutcome
            {
               OutType = "EDU"
            };

            NewRule().OutTypeConditionMet(dpOutcome).Should().BeTrue();
        }

        [Theory]
        [InlineData("XXX")]
        [InlineData(null)]
        public void OutTypeConditionMet_False(string type)
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = type
            };

            NewRule().OutTypeConditionMet(dpOutcome).Should().BeFalse();
        }

        [Fact]
        public void OutCodeConditionMet_True()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = "EDU",
                OutCode = 10
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(false);

            NewRule(lookupsMock.Object).OutCodeConditionMet(dpOutcome).Should().BeTrue();
        }

        [Fact]
        public void OutCodeConditionMet_False()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = "EDU",
                OutCode = 1
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(true);

            NewRule(lookupsMock.Object).OutCodeConditionMet(dpOutcome).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = "EDU",
                OutCode = 10
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(false);

            NewRule(lookupsMock.Object).ConditionMet(dpOutcome).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var dpOutcome = new TestDPOutcome
            {
                OutType = "XXX",
                OutCode = 10
            };

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(false);

            NewRule(lookupsMock.Object).ConditionMet(dpOutcome).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutType = "EDU",
                        OutCode = 10
                    }
                }
            };

            var dpOutcome = learnerDP.DPOutcomes.FirstOrDefault();

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learnerDP = new TestLearnerDestinationAndProgression
            {
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutType = "EDU",
                        OutCode = 1
                    }
                }
            };

            var dpOutcome = learnerDP.DPOutcomes.FirstOrDefault();

            var lookupsMock = new Mock<IProvideLookupDetails>();

            lookupsMock.Setup(ds => ds.Contains(LookupTimeRestrictedKey.OutTypedCode, $"{dpOutcome.OutType}{dpOutcome.OutType}")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(lookupsMock.Object, validationErrorHandlerMock.Object).Validate(learnerDP);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OutType", "GAP")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("GAP");

            validationErrorHandlerMock.Verify();
        }

        private OutType_01Rule NewRule(IProvideLookupDetails lookups = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new OutType_01Rule(lookups, validationErrorHandler);
        }
    }
}
