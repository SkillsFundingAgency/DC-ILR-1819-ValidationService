using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.SSN
{
    public class SSN_02Rule : AbstractRule, IRule<ILearner>
    {
        private const string _regexString = @"^[A-HJ-NPR-Y]{4}[0-9]{8}[A-HJ-NPR-Z]{1}$";
        private const int _startingCalculatedValue = 0;
        private const int _startingWeightFactor = 13;
        private const int _checkSumConstant = 23;

        private readonly Regex _regex = new Regex(_regexString, RegexOptions.Compiled);

        public SSN_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SSN_02)
        {
        }

        public enum ValidLetters
        {
            A, B, C, D, E, F, G, H, J, K, L, M, N, P, R, S, T, U, V, W, X, Y, Z
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (string.IsNullOrWhiteSpace(learningDelivery.LearningDeliveryHEEntity?.SSN))
                {
                    continue;
                }

                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity.SSN.ToUpper()))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.SSN));
                }
            }
        }

        public bool ConditionMet(string ssn)
        {
            return !RegexIsValid(ssn) || !CheckSumMatchesExpected(ssn);
        }

        public bool RegexIsValid(string ssn)
        {
            return _regex.IsMatch(ssn);
        }

        public bool CheckSumMatchesExpected(string ssn)
        {
            var expectedCheckSum = ssn.Substring(ssn.Length - 1);

            var checkSum = GetLetter(GetCalculatedCheckSumValue(ssn));

            return !string.IsNullOrEmpty(checkSum) && checkSum.Equals(expectedCheckSum);
        }

        public int GetCalculatedCheckSumValue(string ssn)
        {
            /* Check sum algorithm break down can be found here:
               https://www.hesa.ac.uk/collection/c18051/a/ssn */

            char[] ssn1To4 = ssn.Substring(0, 4).ToCharArray();
            int[] ssn5To12 = ssn.Substring(4, 8).Select(c => Convert.ToInt32(c.ToString())).ToArray();

            var calculatedValues = _startingCalculatedValue;
            var weightingFactor = _startingWeightFactor;

            foreach (var letter in ssn1To4)
            {
                var letterValue = GetLetterValue(letter.ToString());
                var calculatedValue = letterValue * weightingFactor;

                calculatedValues += calculatedValue;
                weightingFactor -= 1;
            }

            foreach (var number in ssn5To12)
            {
                var calculatedValue = number * weightingFactor;

                calculatedValues += calculatedValue;
                weightingFactor -= 1;
            }

            var remainder = calculatedValues % _checkSumConstant;

            var checkSumValue = _checkSumConstant - remainder - 1; // -1 as the last position value set starts at '1' and contains an extra letter 'Z'

            return checkSumValue;
        }

        public string GetLetter(int value)
        {
            if (Enum.IsDefined(typeof(ValidLetters), value))
            {
                return Enum.Parse(typeof(ValidLetters), value.ToString()).ToString();
            }

            return null;
        }

        public int GetLetterValue(string letter)
        {
            return (int)Enum.Parse(typeof(ValidLetters), letter);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string ssn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.SSN, ssn)
            };
        }
    }
}
