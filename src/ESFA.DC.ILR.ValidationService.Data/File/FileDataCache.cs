using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.File
{
    public class FileDataCache : IFileDataCache
    {
        public DateTime FilePreparationDate { get; set; }

        public int UKPRN { get; set; }

        public IEnumerable<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions { get; set; }

        public string FileName { get; set; }

        public int? FileNameUKPRN
        {
            get
            {
                try
                {
                    return int.Parse(FileName.Split('-')[1]);
                }
                catch (Exception)
                {
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the learners.
        /// TODO: consider the wisdom of this....
        /// </summary>
        public IReadOnlyCollection<ILearner> Learners { get; set; }
    }
}
