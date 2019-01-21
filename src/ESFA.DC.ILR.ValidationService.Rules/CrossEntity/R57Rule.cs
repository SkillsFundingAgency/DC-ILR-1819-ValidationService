﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R57Rule : AbstractRule, IRule<ILearner>
    {
        public R57Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R57)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var nonFundedOpenMainAims =
                objectToValidate.LearningDeliveries
                    .Where(ld => ld.FundModel == TypeOfFunding.NotFundedByESFA &&
                                 ld.AimType == TypeOfAim.ProgrammeAim &&
                                 !ld.LearnActEndDateNullable.HasValue).ToList();

            foreach (var nonFundedMainAim in nonFundedOpenMainAims)
            {
                var openFundedComponentAims = objectToValidate.LearningDeliveries.Where(x =>
                                  x.AimType == TypeOfAim.ComponentAimInAProgramme &&
                                  x.FundModel != TypeOfFunding.NotFundedByESFA &&
                                  x.ProgTypeNullable == nonFundedMainAim.ProgTypeNullable &&
                                  x.FworkCodeNullable == nonFundedMainAim.FworkCodeNullable &&
                                  x.PwayCodeNullable == nonFundedMainAim.PwayCodeNullable &&
                                   x.StdCodeNullable == nonFundedMainAim.StdCodeNullable);

                foreach (var openFundedComponentAim in openFundedComponentAims)
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        openFundedComponentAim.AimSeqNumber,
                        BuildErrorMessageParameters(openFundedComponentAim));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, learningDelivery.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, learningDelivery.FworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, learningDelivery.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, learningDelivery.StdCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable)
            };
        }
    }
}
