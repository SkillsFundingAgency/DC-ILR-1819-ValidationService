using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract
{
    public abstract class AbstractRuleTests<T>
        where T : class
    {
        protected ValidationErrorHandlerMock BuildValidationErrorHandlerMockForError()
        {
            var validationErrorHandlerMock = new ValidationErrorHandlerMock(true);

            validationErrorHandlerMock.Setup(veh => veh.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>())).Verifiable();

            return validationErrorHandlerMock;
        }

        protected ValidationErrorHandlerMock BuildValidationErrorHandlerMockForNoError()
        {
            var validationErrorHandlerMock = new ValidationErrorHandlerMock(false);

            validationErrorHandlerMock
                .Setup(veh => veh.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()))
                .Throws(new Exception("Validation Error should not be Handled."));

            return validationErrorHandlerMock;
        }

        protected Mock<T> NewRuleMock()
        {
            return new Mock<T>
            {
                CallBase = true
            };
        }

        protected void VerifyErrorHandlerMock(ValidationErrorHandlerMock errorHandlerMock, int times = 0)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Exactly(times));
        }
    }
}
