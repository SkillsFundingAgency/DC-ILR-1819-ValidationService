using System;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks
{
    public class ValidationErrorHandlerMock : Mock<IValidationErrorHandler>, IDisposable
    {
        private readonly bool _verify;

        public ValidationErrorHandlerMock(bool verify)
        {
            _verify = verify;
        }

        public void Dispose()
        {
            if (_verify)
            {
                this.VerifyAll();
            }
        }
    }
}
