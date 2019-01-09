using System;
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
    public class R104Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = LearningDeliveryFAMTypeConstants.ACT;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private DateTime? _errorDateFrom;
        private DateTime? _errorDateTo;

        public R104Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R104)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnPlanEndDate,
                            learningDelivery.LearnActEndDateNullable,
                            _famTypeACT,
                            _errorDateFrom,
                            _errorDateTo));
                }
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ACTCountConditionMet(learningDeliveryFAMs)
                   && ACTDateConditionMet(learningDeliveryFAMs);
        }

        public bool ACTCountConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                return _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs as IReadOnlyCollection<ILearningDeliveryFAM>, _famTypeACT) > 1;
            }

            return false;
        }

        public bool ACTDateConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (learningDeliveryFAMs != null)
            {
                var fams = learningDeliveryFAMs.Where(f => f.LearnDelFAMType == _famTypeACT).OrderBy(ldf => ldf.LearnDelFAMDateFromNullable).ToArray();
                var famCount = fams.Count();

                if (famCount == 0)
                {
                    return false;
                }

                var i = 0;

                while (i < famCount)
                {
                    var condition =
                        fams[i].LearnDelFAMDateToNullable == null || fams[i + 1].LearnDelFAMDateFromNullable == null
                        ? false
                        : fams[i].LearnDelFAMDateToNullable >= fams[i + 1].LearnDelFAMDateFromNullable == true;

                    if (condition == true)
                    {
                        _errorDateFrom = fams[i].LearnDelFAMDateFromNullable;
                        _errorDateTo = fams[i].LearnDelFAMDateToNullable;
                        return true;
                    }

                    i++;
                }

                return false;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate, DateTime? learnActEndDate, string famType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
