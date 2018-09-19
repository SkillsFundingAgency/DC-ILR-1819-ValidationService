using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_59Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 08, 01);
        private readonly DateTime _endJuly2016 = new DateTime(2016, 07, 31);
        private readonly int _larsCategoryRef = 19;
        private readonly HashSet<int> _priorAttains = new HashSet<int> { 3, 4, 5, 10, 11, 12, 13, 97, 98 };

        private readonly IDD07 _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnAimRef_59Rule(IDD07 dd07, IDateTimeQueryService dateTimeQueryService, ILARSDataService larsDataService, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_59)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _larsDataService = larsDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public LearnAimRef_59Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef,
                    objectToValidate.DateOfBirthNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnAimRef,
                            objectToValidate.DateOfBirthNullable,
                            learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? priorAttain, int? progType, string learnAimRef, DateTime? dateOfBirth, DateTime learnStartDate,  IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && DD07ConditionMet(progType)
                && LearnStartDateConditionMet(learnStartDate)
                && AgeConditionMet(dateOfBirth, learnStartDate)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs)
                && LARSCategoryConditionMet(learnAimRef)
                && LARSNVQLevel2ConditionMet(learnAimRef)
                && Level3QualificationConditionMet(priorAttain, learnAimRef, learnStartDate);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public virtual bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAugust2015 && learnStartDate <= _endJuly2016;
        }

        public virtual bool AgeConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) >= 24;
        }

        public virtual bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "346")
                && !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")
                && !_learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES");
        }

        public virtual bool LARSCategoryConditionMet(string learnAimRef)
        {
            return !_larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, _larsCategoryRef);
        }

        public virtual bool LARSNVQLevel2ConditionMet(string learnAimRef)
        {
            return _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "3");
        }

        public virtual bool Level3QualificationConditionMet(int? priorAttain, string learnAimRef, DateTime learnStartDate)
        {
            return _larsDataService.EffectiveDatesValidforLearnAimRef(learnAimRef, learnStartDate)
                && ((priorAttain.HasValue && _priorAttains.Contains(priorAttain.Value))
                || !_larsDataService.FullLevel3PercentForLearnAimRefAndDateAndPercentValue(learnAimRef, learnStartDate, 100m));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
            };
        }
    }
}
