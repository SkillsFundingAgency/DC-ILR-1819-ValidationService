using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface
{
    public interface IFileDataService
    {
        int UKPRN();

        DateTime FilePreparationDate();

        IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions();

        ILearnerDestinationAndProgression LearnerDestinationAndProgressionsForLearnRefNumber(string learnRefNumber);

        string FileName();

        int? FileNameUKPRN();
    }
}
