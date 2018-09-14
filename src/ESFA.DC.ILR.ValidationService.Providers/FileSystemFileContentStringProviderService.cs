using System.IO;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FileSystemFileContentStringProviderService : IMessageStreamProviderService
    {
        private readonly IPreValidationContext _preValidationContext;

        public FileSystemFileContentStringProviderService(IPreValidationContext preValidationContext)
        {
            _preValidationContext = preValidationContext;
        }

        public Stream Provide()
        {
            return File.Open(_preValidationContext.Input, FileMode.Open);
        }
    }
}
