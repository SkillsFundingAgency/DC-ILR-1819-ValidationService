using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File
{
    public class FileDataCache : IFileDataCache
    {
        public DateTime FilePreparationDate { get; set; }

        public int UKPRN { get; set; }

        public IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions { get; set; }
    }
}
