using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R64Rule : AbstractRule, IRule<ILearner>
    {
        private const int ValidOutcome = 1;
        private const int ExcludedProgType = 25;
        private const int ValidCompStatus = 2;
        private readonly ILARSDataService _larsDataService;
        private readonly HashSet<int> ValidFundModels = new HashSet<int>() { 35, 36 };
        private readonly HashSet<int?> ValidComponentTypes = new HashSet<int?>() { 1, 3 };

        public R64Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R64)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var filteredLearningDeliveries = objectToValidate.LearningDeliveries
                .Where(x => AimTypeConditionMet(x.AimType) &&
                            FundModelsConditionMet(x.FundModel) &&
                            !ExcludeConditionMet(x.FundModel) &&
                            LarsComponentTypeConditionMet(x.LearnAimRef));

            var completedLearningDeliveries = filteredLearningDeliveries.
                Where(x => CompletedLearningDeliveryConditionMet(x.CompStatus, x.OutcomeNullable));

            if (completedLearningDeliveries.Any())
            {
                foreach (var completedLearningDelivery in completedLearningDeliveries)
                {
                    if (ConditionMet(filteredLearningDeliveries, completedLearningDelivery))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, completedLearningDelivery.AimSeqNumber);
                    }
                }
            }
        }

        public bool CompletedLearningDeliveryConditionMet(int compStatus, int? outCome)
        {
            return compStatus == ValidCompStatus && outCome.HasValue && outCome == ValidOutcome;
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery completedLearningDelivery)
        {
            return learningDeliveries.Any(
                x => x.ProgTypeNullable == completedLearningDelivery.ProgTypeNullable &&
                     x.FworkCodeNullable == completedLearningDelivery.FworkCodeNullable &&
                     x.PwayCodeNullable == completedLearningDelivery.PwayCodeNullable &&
                     x.LearnStartDate > completedLearningDelivery.LearnStartDate);
        }

        public bool FundModelsConditionMet(int fundModel)
        {
            return ValidFundModels.Contains(fundModel);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == TypeOfAim.ComponentAimInAProgramme;
        }

        public bool LarsComponentTypeConditionMet(string learnAimRef)
        {
            return _larsDataService.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, ValidComponentTypes);
        }

        public bool ExcludeConditionMet(int? progType)
        {
            return progType.HasValue && progType == ExcludedProgType;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
           ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, learningDelivery.FworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, learningDelivery.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, learningDelivery.StdCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, learningDelivery.OutcomeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, learningDelivery.CompStatus)
            };
        }
    }
}
