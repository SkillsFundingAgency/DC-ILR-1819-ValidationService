using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface
{
    public interface IFileDataService
    {
        int UKPRN();

        DateTime FilePreparationDate();

        IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions();

        ILearnerDestinationAndProgression LearnerDestinationAndProgressionsForLearnRefNumber(string learnRefNumber);
    }
}
