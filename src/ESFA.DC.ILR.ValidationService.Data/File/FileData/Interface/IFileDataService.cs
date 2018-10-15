using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Get learners.
        /// </summary>
        /// <param name="usingRestriction">using restriction.</param>
        /// <returns>a subset of learners based on the incoming restriction</returns>
        IReadOnlyCollection<ILearner> GetLearners(Func<ILearner, bool> usingRestriction);

        /// <summary>
        /// Gets destination and progressions.
        /// </summary>
        /// <param name="usingRestriction">using restriction.</param>
        /// <returns>a subset of destination and progressions based on the incoming restriction</returns>
        IReadOnlyCollection<ILearnerDestinationAndProgression> GetDestinationAndProgressions(Func<ILearnerDestinationAndProgression, bool> usingRestriction);
    }
}
