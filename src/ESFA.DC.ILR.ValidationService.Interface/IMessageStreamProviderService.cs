using System.IO;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IMessageStreamProviderService
    {
        Stream Provide();
    }
}
