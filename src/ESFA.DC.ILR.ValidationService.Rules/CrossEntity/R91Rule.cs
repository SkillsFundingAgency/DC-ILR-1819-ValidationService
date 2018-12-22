using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R91Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModel = TypeOfFunding.EuropeanSocialFund;
        private HashSet<string> _contractReferencesCompleted = new HashSet<string>();

        public R91Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R91)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            bool isFundModelPresent = false;
            bool isSameContractReferencePresent = false;
            string conRefNumberESFLearningAim = string.Empty;
            int compStatusESFLearningAim = CompletionState.HasCompleted;
            int aimSequenceNumber = 0;

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (FundModelConditionMet(learningDelivery.FundModel))
                {
                    isFundModelPresent = true;
                    conRefNumberESFLearningAim = learningDelivery.ConRefNumber;
                    compStatusESFLearningAim = learningDelivery.CompStatus;
                    aimSequenceNumber = learningDelivery.AimSeqNumber;
                    if (LearnAimRefConditionMet(learningDelivery.LearnAimRef)
                        && CompStatusConditionMet(learningDelivery.CompStatus))
                    {
                        if (ContractReferenceConditionMet(learningDelivery.ConRefNumber))
                        {
                            isSameContractReferencePresent = true;
                            break;
                        }

                        _contractReferencesCompleted.Add(learningDelivery.ConRefNumber);
                    }
                }
            }

            // if fund model is present but no esf learning aim present
            // and no same contract reference completed aims
            if (isFundModelPresent
                && !isSameContractReferencePresent)
            {
                HandleValidationError(
                            learnRefNumber: objectToValidate.LearnRefNumber,
                            aimSequenceNumber: aimSequenceNumber,
                            errorMessageParameters: BuildErrorMessageParameters(
                            _fundModel,
                            conRefNumberESFLearningAim,
                            compStatusESFLearningAim));
            }
        }

        public bool FundModelConditionMet(int fundModel) => _fundModel == fundModel;

        public bool LearnAimRefConditionMet(string learnAimRef) => learnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment;

        public bool CompStatusConditionMet(int compStatus) => compStatus == CompletionState.HasCompleted;

        public bool ContractReferenceConditionMet(string conRefNumber) => _contractReferencesCompleted.Contains(conRefNumber);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
