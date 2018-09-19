using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IFileDataCache
    {
        DateTime FilePreparationDate { get; }

        string FileName { get; set; }

        int? FileNameUKPRN { get; }

        int UKPRN { get; }

        IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions { get; }
    }
}
