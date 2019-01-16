using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_80Rule : AbstractRule, IRule<ILearner>
    {
        private const string _legalOrgType = "USDC";
        private readonly DateTime _learnStartDateMinimum = new DateTime(2016, 8, 1);
        private readonly DateTime _learnStartDateMaximum = new DateTime(2017, 7, 31);
        private readonly IEnumerable<int?> _priorAttains = new HashSet<int?>() { 3, 4, 5, 10, 11, 12, 13, 97, 98 };

        private readonly ILARSDataService _larsDataService;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IDerivedData_07Rule _dd07;
        private readonly IFileDataService _fileDataService;

        public LearnAimRef_80Rule(
            ILARSDataService larsDataService,
            IOrganisationDataService organisationDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IDerivedData_07Rule dd07,
            IFileDataService fileDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_80)
        {
            _larsDataService = larsDataService;
            _organisationDataService = organisationDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
            _dd07 = dd07;
            _fileDataService = fileDataService;
        }

        public LearnAimRef_80Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    ukprn,
                    learningDelivery.FundModel,
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            objectToValidate.PriorAttainNullable,
                            learningDelivery.LearnAimRef,
                            learningDelivery.LearnStartDate,
                            learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int ukprn, int fundModel, int? priorAttain, DateTime learnStartDate, int? progType, string learnAimRef, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return FundModelConditionMet(fundModel)
                   && PriorAttainmentConditionMet(priorAttain)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ApprenticeshipConditionMet(progType)
                   && LevelConditionMet(learnAimRef)
                   && OrganisationConditionMet(ukprn)
                   && RestartConditionMet(learningDeliveryFams);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _learnStartDateMinimum && learnStartDate <= _learnStartDateMaximum;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public virtual bool PriorAttainmentConditionMet(int? priorAttain)
        {
            return _priorAttains.Contains(priorAttain);
        }

        public virtual bool LevelConditionMet(string learnAimRef)
        {
            return _larsDataService.NotionalNVQLevelV2MatchForLearnAimRefAndLevel(learnAimRef, "3");
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public virtual bool RestartConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES);
        }

        public virtual bool OrganisationConditionMet(int ukprn)
        {
            return !_organisationDataService.LegalOrgTypeMatchForUkprn(ukprn, _legalOrgType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? priorAttain, string learnAimRef, DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, priorAttain),
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
