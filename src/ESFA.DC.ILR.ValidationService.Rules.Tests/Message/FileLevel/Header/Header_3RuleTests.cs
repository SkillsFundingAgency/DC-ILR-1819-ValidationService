using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Message.FileLevel.Header
{
    public class Header_3RuleTests : AbstractRuleTests<Header_3Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Header_3");
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(55556666, 55556666).Should().BeFalse();
        }

        [Theory]
        [InlineData(1122222, null)]
        [InlineData(1122222, 333355555)]
        public void ConditionMet_True(int headerUKPRN, int? fileNameUKPRN)
        {
            NewRule().ConditionMet(headerUKPRN, fileNameUKPRN).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    SourceEntity = new TestSource()
                    {
                        UKPRN = 123456
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(fds => fds.FileName()).Returns("ILR_12345678");
            fileDataServiceMock.Setup(fds => fds.FileNameUKPRN()).Returns(456789);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var message = new TestMessage()
            {
                HeaderEntity = new TestHeader()
                {
                    SourceEntity = new TestSource()
                    {
                        UKPRN = 123456
                    }
                }
            };

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(fds => fds.FileName()).Returns("ILR_123456");
            fileDataServiceMock.Setup(fds => fds.FileNameUKPRN()).Returns(123456);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, fileDataService: fileDataServiceMock.Object).Validate(message);
            }
        }

        public Header_3Rule NewRule(
            IFileDataService fileDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new Header_3Rule(
                fileDataService: fileDataService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
