using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData
{
    public class FileDataService :
        IFileDataService
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

        /// <summary>
        /// Gets learners.
        /// </summary>
        /// <param name="usingRestriction">using restriction.</param>
        /// <returns>a subset of learners based on the incoming restriction</returns>
        public IReadOnlyCollection<ILearner> GetLearners(Func<ILearner, bool> usingRestriction)
        {
            return _fileDataCache.Learners
                .SafeWhere(usingRestriction)
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Gets destination and progressions.
        /// </summary>
        /// <param name="usingRestriction">using restriction.</param>
        /// <returns>a subset of destination and progressions based on the incoming restriction</returns>
        public IReadOnlyCollection<ILearnerDestinationAndProgression> GetDestinationAndProgressions(Func<ILearnerDestinationAndProgression, bool> usingRestriction)
        {
            return _fileDataCache.LearnerDestinationAndProgressions
                .SafeWhere(usingRestriction)
                .AsSafeReadOnlyList();
        }

        public ILearnerDestinationAndProgression LearnerDestinationAndProgressionsForLearnRefNumber(string learnRefNumber)
        {
            return _fileDataCache.LearnerDestinationAndProgressions?
                .Where(dp => dp.LearnRefNumber == learnRefNumber)
                .Select(dp => dp).FirstOrDefault();
        }

        public string FileName()
        {
            return _fileDataCache.FileName;
        }

        public int? FileNameUKPRN()
        {
            return _fileDataCache.FileNameUKPRN;
        }
    }
}
