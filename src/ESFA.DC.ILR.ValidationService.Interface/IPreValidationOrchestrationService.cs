using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<U>
    {
        Task ExecuteAsync(IPreValidationContext validationContext, CancellationToken cancellationToken);
    }
}