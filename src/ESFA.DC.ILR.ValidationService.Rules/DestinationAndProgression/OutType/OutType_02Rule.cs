using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType
{
    public class OutType_02Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly HashSet<string> _outTypes = new HashSet<string>
        {
            OutTypeConstants.PaidEmployment,
            OutTypeConstants.GapYear,
            OutTypeConstants.NotInPaidEmployment,
            OutTypeConstants.Other,
            OutTypeConstants.SocialDestination,
            OutTypeConstants.VoluntaryWork
        };

        private readonly ILearnerDPQueryService _learnerDPQueryService;
        private string _outTypesForError;

        public OutType_02Rule(ILearnerDPQueryService learnerDPQueryService, IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.OutType_02)
        {
            _learnerDPQueryService = learnerDPQueryService;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            var outTypesList = _learnerDPQueryService.OutTypesForStartDateAndTypes(objectToValidate.DPOutcomes, _outTypes);

            if (outTypesList.Any())
            {
                foreach (var outTypeList in outTypesList)
                {
                    if (ConditionMet(outTypeList.Value))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(outTypeList.Key, _outTypesForError));
                    }
                }
            }
        }

        public bool ConditionMet(IEnumerable<string> outTypes)
        {
            var repeatedOutTypes = outTypes
                .GroupBy(x => x)
                 .Where(g => g.Count() > 1)
                     .Select(y => y.Key)
                     .Where(d => _outTypes.Contains(d))
                 .ToList();

            _outTypesForError = string.Join(", ", repeatedOutTypes);

            return repeatedOutTypes.Count() < 1 ? false : true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? outstartDate, string outTypes)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outstartDate),
                BuildErrorMessageParameter(PropertyNameConstants.OutType, outTypes),
            };
        }
    }
}
