﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int?> _englishPrescribedIDs = new HashSet<int?>() { 1, 2 };
        private readonly HashSet<int?> _allowedProgTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.HigherApprenticeshipLevel4,
            TypeOfLearningProgramme.HigherApprenticeshipLevel5,
            TypeOfLearningProgramme.HigherApprenticeshipLevel6,
            TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus,
            TypeOfLearningProgramme.ApprenticeshipStandard
        };

        private readonly IFileDataService _fileDataService;
        private readonly ILARSDataService _lARSDataService;
        private readonly IDerivedData_27Rule _derivedData_27Rule;
        private readonly IOrganisationDataService _organisationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearningDeliveryHE_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            ILARSDataService lARSDataService,
            IDerivedData_27Rule derivedData_27Rule,
            IOrganisationDataService organisationDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearningDeliveryHE_07)
        {
            _fileDataService = fileDataService;
            _lARSDataService = lARSDataService;
            _derivedData_27Rule = derivedData_27Rule;
            _organisationDataService = organisationDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                || !DerivedData27ConditionMet(_fileDataService.UKPRN())
                || _organisationDataService.LegalOrgTypeMatchForUkprn(_fileDataService.UKPRN(), LegalOrgTypeConstants.UHEO))
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef,
                    learningDelivery.LearningDeliveryHEEntity,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(int? progTypeNullable, string learnAimRef, ILearningDeliveryHE learningDeliveryHEEntity, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ProgTypeConditionMet(progTypeNullable)
                && LearningDeliveryHEConditionMet(learningDeliveryHEEntity)
                && FAMSConditionMet(learningDeliveryFAMs)
                && LARSConditionMet(learnAimRef);
        }

        public bool FAMSConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
            => !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(
            learningDeliveryFAMs,
            LearningDeliveryFAMTypeConstants.LDM,
            "352");

        public bool LARSConditionMet(string learnAimRef) => _lARSDataService.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, _englishPrescribedIDs);

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHEEntity) => learningDeliveryHEEntity == null;

        public bool DerivedData27ConditionMet(int ukprn) => _derivedData_27Rule.IsUKPRNCollegeOrGrantFundedProvider(ukprn);

        public bool ProgTypeConditionMet(int? progTypeNullable) => _allowedProgTypes.Contains(progTypeNullable);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progTypeNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progTypeNullable)
            };
        }
    }
}
