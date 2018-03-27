﻿using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_24Rule : AbstractRule, IRule<ILearner>
    {
        public DateOfBirth_24Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.ULNNullable, objectToValidate.DateOfBirthNullable))
            {
                HandleValidationError(RuleNameConstants.DateOfBirth_24, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? uln, DateTime? dateOfBirth)
        {
            return uln.HasValue
                && uln != ValidationConstants.TemporaryULN
                && !dateOfBirth.HasValue;
        }
    }
}