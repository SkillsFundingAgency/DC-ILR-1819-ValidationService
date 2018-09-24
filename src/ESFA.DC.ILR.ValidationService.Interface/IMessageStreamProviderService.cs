using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IMessageStreamProviderService
    {
        Task<Stream> Provide(CancellationToken cancellationToken);
    }
}
