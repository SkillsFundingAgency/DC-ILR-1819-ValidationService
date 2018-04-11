using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler
{
    public class ValidationErrorHandlerTests
    {
        [Fact]
        public void Handle()
        {
            var validationErrorHandler = NewHandler();

            validationErrorHandler.Handle("RuleName");

            validationErrorHandler.ErrorBag.Should().HaveCount(1);
        }

        [Fact]
        public void HandleParallel()
        {
            var validationErrorHandler = NewHandler();

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
            var validationErrorHandler = NewHandler();

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
            var validationErrorHandler = NewHandler();

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
            var validationErrorHandler = NewHandler();

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
            var validationErrorHandler = NewHandler();

            validationErrorHandler.Handle("RuleName", "LearnRefNumber", 1234, new List<IErrorMessageParameter>() { new ErrorMessageParameter("A", "error"), new ErrorMessageParameter("B", "message"), new ErrorMessageParameter("C", "parameters") });

            var validationError = validationErrorHandler.ErrorBag.First();

            validationError.RuleName.Should().Be("RuleName");
            validationError.LearnerReferenceNumber.Should().Be("LearnRefNumber");
            validationError.AimSequenceNumber.Should().Be(1234);
            validationError.ErrorMessageParameters.Should().HaveCount(3);
            validationError.ErrorMessageParameters.Should().Equal(new List<IErrorMessageParameter>() { new ErrorMessageParameter("A", "error"), new ErrorMessageParameter("B", "message"), new ErrorMessageParameter("C", "parameters") });
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", "Value");

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be("Value");
        }

        [Fact]
        public void BuildErrorMessageParameters_ToString()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", 123);

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be("123");
        }

        [Fact]
        public void BuildErrorMessageParameters_Null()
        {
            var errorMessageParameter = NewHandler().BuildErrorMessageParameter("Property", null);

            errorMessageParameter.PropertyName.Should().Be("Property");
            errorMessageParameter.Value.Should().Be(null);
        }

        private ValidationErrorHandler NewHandler()
        {
            return new ValidationErrorHandler();
        }
    }
}
