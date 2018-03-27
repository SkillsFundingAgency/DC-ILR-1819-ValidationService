using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If the FAM type is NLM, EDF or PPE there must be no more than two occurrences
    /// </summary>
    public class LearnFAMType_11Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _famTypesToCheck = new HashSet<string>()
        {
            LearnerFamTypeConstants.NLM,
            LearnerFamTypeConstants.EDF,
            LearnerFamTypeConstants.PPE
        };

        public LearnFAMType_11Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learnerFam in objectToValidate.LearnerFAMs.GroupBy(x => x.LearnFAMType).Select(x => x.Key))
            {
                if (ConditionMet(learnerFam, objectToValidate.LearnerFAMs))
                {
                    HandleValidationError(RuleNameConstants.LearnFAMType_11Rule, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(string famType, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return !string.IsNullOrWhiteSpace(famType) &&
                   FamTypesListCheckConditionMet(famType) &&
                   FamTypeCountConditionMet(famType, learnerFams);
        }

        public bool FamTypesListCheckConditionMet(string famType)
        {
            return _famTypesToCheck.Contains(famType);
        }

        public bool FamTypeCountConditionMet(string famType, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return learnerFams.Count(x => x.LearnFAMType == famType) > 2;
        }
    }
}