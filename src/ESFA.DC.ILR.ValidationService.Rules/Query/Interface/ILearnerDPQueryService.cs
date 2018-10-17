using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerDPQueryService
    {
        bool HasULNForLearnRefNumber(string learnRefNumber, long uln, ILearnerDestinationAndProgression learnerDestinationAndProgression);

        IDictionary<DateTime, IEnumerable<string>> OutTypesForStartDateAndTypes(IEnumerable<IDPOutcome> dpOutcomes, IEnumerable<string> outTypes);
    }
}
