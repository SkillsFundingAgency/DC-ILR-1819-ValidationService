using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R85Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataCache _fileDataCache;
        private readonly ILearnerDPQueryService _learnerDPQueryService;

        public R85Rule(IFileDataCache fileDataCache, ILearnerDPQueryService learnerDPQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R85)
        {
            _fileDataCache = fileDataCache;
            _learnerDPQueryService = learnerDPQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LearnRefNumber, objectToValidate.ULN))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
            }
        }

        public bool ConditionMet(string learnRefNumber, long uln)
        {
            if (_fileDataCache.LearnerDestinationAndProgressions != null)
            {
                var learnerDP = _fileDataCache.LearnerDestinationAndProgressions.Where(l => l.LearnRefNumber == learnRefNumber).FirstOrDefault();

                return learnerDP == null ? false : !_learnerDPQueryService.HasULNForLearnRefNumber(learnRefNumber, uln, learnerDP);
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln)
            };
        }
    }
}
