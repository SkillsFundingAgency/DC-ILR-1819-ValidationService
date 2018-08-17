using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R85Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly ILearnerDPQueryService _learnerDPQueryService;

        public R85Rule(IFileDataService fileDataService, ILearnerDPQueryService learnerDPQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R85)
        {
            _fileDataService = fileDataService;
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
            var learnerDP = _fileDataService.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber);

            return learnerDP == null ? false : !_learnerDPQueryService.HasULNForLearnRefNumber(learnRefNumber, uln, learnerDP);
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
