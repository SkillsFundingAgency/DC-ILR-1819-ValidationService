using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If a FAM type is returned, the FAM code must be a valid lookup for that FAM type
    /// </summary>
    public class LearnFAMType_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _famTypesToCheck = new HashSet<string>()
        {
            LearnerFamTypeConstants.HNS,
            LearnerFamTypeConstants.EHC,
            LearnerFamTypeConstants.DLA,
            LearnerFamTypeConstants.SEN,
            LearnerFamTypeConstants.MCF,
            LearnerFamTypeConstants.ECF,
            LearnerFamTypeConstants.FME
        };

        public LearnFAMType_09Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learnerFam in objectToValidate.LearnerFAMs.GroupBy(x => x.LearnFAMType).Select(x => x.Key))
            {
                if (ConditionMet(learnerFam, objectToValidate.LearnerFAMs))
                {
                    HandleValidationError(RuleNameConstants.LearnFAMType_09Rule, objectToValidate.LearnRefNumber);
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
            return learnerFams.Count(x => x.LearnFAMType == famType) > 1;
        }
    }
}