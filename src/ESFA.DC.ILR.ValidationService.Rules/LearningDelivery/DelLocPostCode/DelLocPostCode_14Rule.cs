using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_14Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IFCSDataService _fcsDataService;
        private readonly IPostcodesDataService _postcodeService;
        private readonly IDerivedData_22Rule _derivedData22;

        private readonly DateTime _ruleEndDate = new DateTime(2017, 7, 31);

        public DelLocPostCode_14Rule(
            IFCSDataService fcsDataService,
            IPostcodesDataService postcodeService,
            IDerivedData_22Rule derivedData22,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_14)
        {
            _fcsDataService = fcsDataService;
            _postcodeService = postcodeService;
            _derivedData22 = derivedData22;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearnStartDate > _ruleEndDate)
                {
                    continue;
                }

                if (learningDelivery.FundModel != FundModel
                    || learningDelivery.LearnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment)
                    || learningDelivery.DelLocPostCode.CaseInsensitiveEquals(ValidationConstants.TemporaryPostCode))
                {
                    continue;
                }

                var localAuthorities = _fcsDataService.GetEligibilityRuleLocalAuthoritiesFor(learningDelivery.ConRefNumber);
                if (localAuthorities == null || localAuthorities.All(la => string.IsNullOrEmpty(la.Code?.Trim())))
                {
                    continue;
                }

                var onsPostCode = _postcodeService.GetONSPostcode(learningDelivery.DelLocPostCode);

                DateTime? latestLearningStart =
                    _derivedData22.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries);

                if (onsPostCode == null ||
                    latestLearningStart == null ||
                    localAuthorities.All(la => !la.Code.CaseInsensitiveEquals(onsPostCode.LocalAuthority)) ||
                    localAuthorities.Any(la => la.Code.CaseInsensitiveEquals(onsPostCode.LocalAuthority)
                                               && (latestLearningStart < onsPostCode.EffectiveFrom
                                               || latestLearningStart > onsPostCode.EffectiveTo
                                               || latestLearningStart >= (onsPostCode.Termination ?? DateTime.MaxValue))))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery));
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, learningDelivery.DelLocPostCode),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, learningDelivery.ConRefNumber)
            };
        }
    }
}
