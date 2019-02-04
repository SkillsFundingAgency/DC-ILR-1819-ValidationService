using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_71Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IFCSDataService _fCSDataService;
        private readonly ILARSDataService _lARSDataService;

        public LearnAimRef_71Rule(
            IValidationErrorHandler validationErrorHandler,
            IFCSDataService fCSDataService,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_71)
        {
            _fCSDataService = fCSDataService;
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ConRefNumber, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(int fundModel, string conRefNumber, string learnAimRef)
        {
            return FundModelConditionMet(fundModel)
                && EsfSectorSubjectAreaLevelConditionMet(conRefNumber)
                && LARSConditionMet(conRefNumber, learnAimRef)
                && LearnAimRefConditionMet(learnAimRef);
        }

        public bool LearnAimRefConditionMet(string learnAimRef) => !learnAimRef.CaseInsensitiveEquals(ValidationConstants.ZESF0001);

        public bool LARSConditionMet(string conRefNumber, string learnAimRef)
        {
            List<decimal?> sectorSubjectAreaCodes =
                _fCSDataService.GetSectorSubjectAreaLevelsFor(conRefNumber)?
                .Where(s => s.SectorSubjectAreaCode.HasValue)
                .Select(s => s.SectorSubjectAreaCode).ToList();

            var learningDeliveries = _lARSDataService.GetDeliveriesFor(learnAimRef);
            bool isMatchNotFoundForSectorSubjectAreaTier1 = !learningDeliveries.Join(sectorSubjectAreaCodes, ld => ld.SectorSubjectAreaTier1, fcs => fcs.Value, (ld, fcs) => fcs.Value).Any();
            bool isMatchNotFoundForSectorSubjectAreaTier2 = !learningDeliveries.Join(sectorSubjectAreaCodes, ld => ld.SectorSubjectAreaTier2, fcs => fcs.Value, (ld, fcs) => fcs.Value).Any();

            return isMatchNotFoundForSectorSubjectAreaTier1 && isMatchNotFoundForSectorSubjectAreaTier2;
        }

        public bool EsfSectorSubjectAreaLevelConditionMet(string conRefNumber) => _fCSDataService.IsSectorSubjectAreaCodeExistsForContract(conRefNumber);

        public bool FundModelConditionMet(int fundModel) => fundModel == _fundModel;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
