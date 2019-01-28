﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN
{
    public class UKPRN_06Rule : AbstractRule, IRule<ILearner>
    {
        private const string _ldm034 = "034";
        private const string _ldm357 = "357";
        private const int _fundModel = TypeOfFunding.AdultSkills;
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEBC1819,
            FundingStreamPeriodCodeConstants.AEB_LS1819,
            FundingStreamPeriodCodeConstants.AEB_TOL1819
        }.ToCaseInsensitiveHashSet();

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IFCSDataService _fcsDataService;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly IAcademicYearQueryService _academicYearQueryService;
        private readonly IDerivedData_07Rule _dd07;

        public UKPRN_06Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IFCSDataService fcsDataService,
            IDerivedData_07Rule dd07,
            IAcademicYearDataService academicYearDataService,
            IAcademicYearQueryService academicYearQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.UKPRN_06)
        {
            _fcsDataService = fcsDataService;
            _academicYearDataService = academicYearDataService;
            _academicYearQueryService = academicYearQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dd07 = dd07;
        }

        public UKPRN_06Rule()
          : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var academicYearStart = _academicYearDataService.Start();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, academicYearStart, learningDelivery.LearnActEndDateNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, DateTime academicYearStart, DateTime? learnActEndDate, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && (learningDeliveryFAMs != null && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs))
                && (progType.HasValue && DD07ConditionMet(progType.Value))
                && (!learnActEndDate.HasValue || LearnActEndDateConditionMet(learnActEndDate.Value, academicYearStart))
                && FCTFundingConditionMet();
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == _fundModel;
        }

        public virtual bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return
                !(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldm034)
                || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, _ldm357));
        }

        public virtual bool DD07ConditionMet(int progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public virtual bool LearnActEndDateConditionMet(DateTime learnActEndDate, DateTime academicYearStart)
        {
            return !_academicYearQueryService.DateIsInPrevAcademicYear(learnActEndDate, academicYearStart);
        }

        public virtual bool FCTFundingConditionMet()
        {
            return !_fcsDataService.FundingRelationshipFCTExists(_fundingStreamPeriodCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
