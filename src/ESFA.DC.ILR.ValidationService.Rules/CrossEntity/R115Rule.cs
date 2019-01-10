using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R115Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int?> _apprenticeshipProgTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.AdvancedLevelApprenticeship,
            TypeOfLearningProgramme.IntermediateLevelApprenticeship,
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus,
        };

        public R115Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R115)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            if (ConditionMet(learner.LearningDeliveries))
            {
                HandleValidationError(learner.LearnRefNumber);
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries
                .Where(IsApprenticeshipProgrammeAim)
                .GroupBy(ld => new { ld.ProgTypeNullable, ld.FworkCodeNullable, ld.PwayCodeNullable })
                .Any(HasDuplicateApprenticeshipFinancialRecord);
        }

        public bool IsApprenticeshipProgrammeAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.AimType == TypeOfAim.ProgrammeAim
                   && learningDelivery.FundModel == TypeOfFunding.ApprenticeshipsFrom1May2017
                   && _apprenticeshipProgTypes.Contains(learningDelivery.ProgTypeNullable);
        }

        public bool HasDuplicateApprenticeshipFinancialRecord(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries
                    .Where(ld => ld.AppFinRecords != null)
                    .SelectMany(ld => ld.AppFinRecords)
                    .GroupBy(afr => new { AFinType = afr.AFinType?.ToUpper(), afr.AFinCode, afr.AFinDate })
                    .Any(afrg => afrg.Count() > 1);
        }
    }
}
