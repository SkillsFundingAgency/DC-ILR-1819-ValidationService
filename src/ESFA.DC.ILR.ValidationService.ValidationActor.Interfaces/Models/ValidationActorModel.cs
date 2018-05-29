namespace ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models
{
    public class ValidationActorModel
    {
        public string JobId { get; set; }

        public byte[] Message { get; set; }

        public byte[] ShreddedLearners { get; set; }

        public byte[] InternalDataCache { get; set; }

        public byte[] ExternalDataCache { get; set; }
    }
}