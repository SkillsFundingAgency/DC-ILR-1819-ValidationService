using System.Collections.Concurrent;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using ESFA.DC.ILR.ValidationService.RuleSet.Output;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler
{
    public class ValidationErrorCachePassThroughOutputServiceTests
    {
        [Fact]
        public void Process_Empty()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorCache<IValidationError>>();

            validationErrorHandlerMock.SetupGet(veh => veh.ValidationErrors).Returns(new ConcurrentBag<IValidationError>());

            var service = NewService(validationErrorHandlerMock.Object);

            var output = service.Process();

            output.Should().BeEmpty();
        }

        [Fact]
        public void Process_Errors()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorCache<IValidationError>>();

            validationErrorHandlerMock.SetupGet(veh => veh.ValidationErrors).Returns(new ConcurrentBag<IValidationError>() { default(ValidationError), default(ValidationError), default(ValidationError) });

            var service = NewService(validationErrorHandlerMock.Object);

            var output = service.Process();

            output.Should().HaveCount(3);
        }

        private IValidationOutputService<IValidationError> NewService(IValidationErrorCache<IValidationError> validationErrorCache = null)
        {
            return new ValidationErrorCachePassThroughOutputService(validationErrorCache);
        }
    }
}
