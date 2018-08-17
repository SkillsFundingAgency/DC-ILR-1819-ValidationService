using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData
{
    public class FileDataService : IFileDataService
    {
        private readonly IFileDataCache _fileDataCache;

        public FileDataService(IFileDataCache fileDataCache)
        {
            _fileDataCache = fileDataCache;
        }

        public int UKPRN()
        {
            return _fileDataCache.UKPRN;
        }

        public DateTime FilePreparationDate()
        {
            return _fileDataCache.FilePreparationDate;
        }

        public IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions()
        {
            return _fileDataCache.LearnerDestinationAndProgressions;
        }

        public ILearnerDestinationAndProgression LearnerDestinationAndProgressionsForLearnRefNumber(string learnRefNumber)
        {
            return _fileDataCache.LearnerDestinationAndProgressions?
                .Where(dp => dp.LearnRefNumber == learnRefNumber)
                .Select(dp => dp).FirstOrDefault();
        }
    }
}
