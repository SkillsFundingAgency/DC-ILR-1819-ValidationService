namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSDataService
    {
        bool FrameworkCodeExists(string learnAimRef, int? progType, int? fworkCode, int? pwayCode);
    }
}
