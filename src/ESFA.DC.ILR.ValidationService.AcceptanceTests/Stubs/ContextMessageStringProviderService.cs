using System.IO;
using System.Text;
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

        public Stream Provide()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(_validationContext.Input));
        }
    }
}
