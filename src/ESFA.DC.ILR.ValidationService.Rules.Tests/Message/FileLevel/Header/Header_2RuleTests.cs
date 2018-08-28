using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Message.FileLevel.Header
{
    public class Header_2RuleTests : AbstractRuleTests<Header_2Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Header_2");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(DateTime.Now.AddDays(1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(DateTime.Now.AddDays(-1)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new TestCollectionDetails()
                    {
                        FilePreparationDate = DateTime.Now.AddDays(1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    CollectionDetailsEntity = new TestCollectionDetails()
                    {
                        FilePreparationDate = DateTime.Now.AddDays(-1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(message);
            }
        }

        public Header_2Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Header_2Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
