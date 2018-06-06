using ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Configuration
{
    public class ReferenceDataOptions : IReferenceDataOptions
    {
        public string LARSConnectionString { get; set; }

        public string PostcodesConnectionString { get; set; }

        public string ULNConnectionstring { get; set; }

        public string ValidationErrorsConnectionString { get; set; }
    }
}
