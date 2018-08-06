using System.IO;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FileSystemFileContentStringProviderService : IMessageStringProviderService
    {
        private readonly IPreValidationContext _preValidationContext;

        public FileSystemFileContentStringProviderService(IPreValidationContext preValidationContext)
        {
            _preValidationContext = preValidationContext;
        }

        public string Provide()
        {
            return File.ReadAllText(_preValidationContext.Input);
        }
    }
}
