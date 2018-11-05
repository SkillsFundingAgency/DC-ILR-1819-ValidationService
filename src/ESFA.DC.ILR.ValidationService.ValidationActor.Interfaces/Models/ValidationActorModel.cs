namespace ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models
{
    public class ValidationActorModel
    {
        public string JobId { get; set; }

        public string Message { get; set; }

        public string InternalDataCache { get; set; }

        public string ExternalDataCache { get; set; }

        public string FileDataCache { get; set; }
    }
}