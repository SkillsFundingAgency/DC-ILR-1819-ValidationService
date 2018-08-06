﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_32Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2016 = new DateTime(2016, 8, 1);
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 8, 1);
        private readonly HashSet<string> _nvqLevel2s = new HashSet<string> { "3", "4", "5", "6", "7", "8", "H" };
        private readonly HashSet<string> _ldmCodes = new HashSet<string> { "034", "346", "347", "339" };

        private readonly IDD07 _dd07;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly ILARSDataService _larsDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly IFileDataCache _fileDataCache;

        public DateOfBirth_32Rule(
            IDD07 dd07,
            IDateTimeQueryService dateTimeQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            ILARSDataService larsDataService,
            IOrganisationDataService organisationDataService,
            IFileDataCache fileDataCache,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_32)
        {
            _dd07 = dd07;
            _dateTimeQueryService = dateTimeQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _larsDataService = larsDataService;
            _organisationDataService = organisationDataService;
            _fileDataCache = fileDataCache;
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataCache.UKPRN;

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    objectToValidate.DateOfBirthNullable,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryFAMs,
                    ukprn))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.FundModel));
                    return;
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime learnStartDate, DateTime? dateOfBirth, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, int ukprn)
        {
            return FundModelConditionMet(fundModel)
                && LearnStartDateConditionMet(learnStartDate)
                && DateOfBirthConditionMet(dateOfBirth, learnStartDate)
                && LARSNotionalNVQL2ConditionMet(learnAimRef)
                && DD07ConditionMet(progType)
                && LARSCategoryConditionMet(learnAimRef)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs)
                && OrganisationTypeConditionMet(ukprn);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _firstAugust2016 && learnStartDate >= _firstAugust2015;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, learnStartDate) >= 24;
        }

        public bool LARSNotionalNVQL2ConditionMet(string learnAimRef)
        {
            return _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _nvqLevel2s);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")
                || _learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, "LDM", _ldmCodes));
        }

        public bool LARSCategoryConditionMet(string learnAimRef)
        {
            return !_larsDataService.LearnAimRefExistsForLearningDeliveryCategoryRef(learnAimRef, 19);
        }

        public bool OrganisationTypeConditionMet(int ukprn)
        {
            return !_organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, "USDC");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}