using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler
{
    public class ValidationErrorHandlerTests
    {
        [Fact]
        public void Handle()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            validationErrorHandler.Handle("RuleName");

            validationErrorHandler.ErrorBag.Should().HaveCount(1);
        }

        [Fact]
        public void HandleParallel()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            Parallel.For(0, 1000, (i) =>
            {
                validationErrorHandler.Handle(i.ToString());
            });

            for (int i = 0; i < 1000; i++)
            {
                validationErrorHandler.ErrorBag.Should().ContainSingle(eb => eb.RuleName == i.ToString());
            }
        }

        [Fact]
        public void Handle_RuleName()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            validationErrorHandler.Handle("RuleName");

            var validationError = validationErrorHandler.ErrorBag.First();

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().BeNull();
            validationError.AimSequenceNumber.Should().BeNull();
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void Handle_LearnRefNumber()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            validationErrorHandler.Handle("RuleName", "LearnRefNumber");

            var validationError = validationErrorHandler.ErrorBag.First();

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().BeNull();
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void Handle_AimSequenceNumber()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            validationErrorHandler.Handle("RuleName", "LearnRefNumber", 1234);

            var validationError = validationErrorHandler.ErrorBag.First();

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.ErrorMessageParameters.Should().BeNull();
        }

        [Fact]
        public void Handle_ErrorMessageParameters()
        {
            var validationErrorHandler = new ValidationErrorHandler();

            validationErrorHandler.Handle("RuleName", "LearnRefNumber", 1234, new string[] { "error", "message", "parameters" });

            var validationError = validationErrorHandler.ErrorBag.First();

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.ErrorMessageParameters.Should().Equal(new string[] { "error", "message", "parameters" });
        }
    }
}
