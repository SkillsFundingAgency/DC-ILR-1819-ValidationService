using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_44Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly IDerivedData_23Rule _derivedDataRule23;

        public DateOfBirth_44Rule(
            IExternalDataCache externalDataCache,
            IDerivedData_23Rule derivedDataRule23,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_44)
        {
            _externalDataCache = externalDataCache;
            _derivedDataRule23 = derivedDataRule23;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.DateOfBirthNullable == null || learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel != TypeOfFunding.EuropeanSocialFund)
                {
                    continue;
                }

                if (_externalDataCache.FCSContractAllocations == null)
                {
                    continue;
                }

                if (!_externalDataCache.FCSContractAllocations.TryGetValue(learningDelivery.ConRefNumber, out var fcsContract)
                    || fcsContract?.EsfEligibilityRule == null)
                {
                    continue;
                }

                int age = _derivedDataRule23.GetLearnersAgeAtStartOfESFContract(learner, learner.LearningDeliveries);
                if (age < (fcsContract.EsfEligibilityRule.MinAge ?? int.MaxValue) || age > (fcsContract.EsfEligibilityRule.MaxAge ?? int.MinValue))
                {
                    RaiseValidationMessage(learner, learningDelivery);
                }
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
