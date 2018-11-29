using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ValidationOutputServiceStub : IValidationOutputService
    {
        public Task ProcessAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
