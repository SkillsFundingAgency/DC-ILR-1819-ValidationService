using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    public interface IFileDataCache
    {
        DateTime FilePreparationDate { get; }

        string FileName { get; set; }

        int? FileNameUKPRN { get; }

        int UKPRN { get; }

        IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions { get; }

        /// <summary>
        /// Gets the learners.
        /// TODO: consider the wisdom of this....
        /// </summary>
        IReadOnlyCollection<ILearner> Learners { get; }
    }
}
