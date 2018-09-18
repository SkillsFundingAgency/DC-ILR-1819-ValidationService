using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ContextMessageStringProviderService : IMessageStreamProviderService
    {
        private readonly IPreValidationContext _validationContext;

        public ContextMessageStringProviderService(IPreValidationContext validationContext)
        {
            _validationContext = validationContext;
        }

        public async Task<Stream> Provide(CancellationToken cancellationToken)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_validationContext.Input));
        }
    }
}
