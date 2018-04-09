using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract
{
    public abstract class AbstractRuleTests
    {
        public ValidationErrorHandlerMock BuildValidationErrorHandlerMockForError(string ruleName, string learnRefNumber = null, long? aimSequenceNumber = 0, IEnumerable<string> errorMessageParameters = null)
        {
            var validationErrorHandlerMock = new ValidationErrorHandlerMock(true);

            validationErrorHandlerMock.Setup(veh => veh.Handle(ruleName, learnRefNumber, aimSequenceNumber, errorMessageParameters)).Verifiable();

            return validationErrorHandlerMock;
        }

        public ValidationErrorHandlerMock BuildValidationErrorHandlerMockForNoError()
        {
            var validationErrorHandlerMock = new ValidationErrorHandlerMock(false);

            validationErrorHandlerMock
                .Setup(veh => veh.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<string>>()))
                .Throws(new Exception("Validation Error should not be Handled."));

            return validationErrorHandlerMock;
        }
    }
}
