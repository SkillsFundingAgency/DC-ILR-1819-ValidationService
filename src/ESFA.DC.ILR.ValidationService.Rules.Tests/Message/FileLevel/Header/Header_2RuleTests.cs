using System;
using ESFA.DC.DateTimeProvider.Interface;
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
        private readonly DateTime utcNow;

        private readonly IDateTimeProvider dateTimeProviderObj;

        public Header_2RuleTests()
        {
            utcNow = DateTime.UtcNow;
            Mock<IDateTimeProvider> dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.GetNowUtc()).Returns(utcNow);
            dateTimeProvider.Setup(x => x.ConvertUkToUtc(It.IsAny<DateTime>())).Returns<DateTime>(d => d);
            dateTimeProviderObj = dateTimeProvider.Object;
        }

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Header_2");
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(utcNow.AddDays(1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(utcNow.AddDays(-1)).Should().BeFalse();
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
                        FilePreparationDate = utcNow.AddDays(1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
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
                        FilePreparationDate = utcNow.AddDays(-1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(message);
            }
        }

        private Header_2Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Header_2Rule(validationErrorHandler, dateTimeProviderObj);
        }
    }
}
