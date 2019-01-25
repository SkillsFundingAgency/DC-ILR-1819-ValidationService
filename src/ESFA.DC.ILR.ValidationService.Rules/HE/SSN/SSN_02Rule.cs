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

        private readonly Dictionary<int, string> _validLetters;
        private readonly Dictionary<string, int> _validLettersInverse;
        private readonly List<Tuple<int, string>> _validLettersList = new List<Tuple<int, string>>
        {
            Tuple.Create(0, "A"),
            Tuple.Create(1, "B"),
            Tuple.Create(2, "C"),
            Tuple.Create(3, "D"),
            Tuple.Create(4, "E"),
            Tuple.Create(5, "F"),
            Tuple.Create(6, "G"),
            Tuple.Create(7, "H"),
            Tuple.Create(8, "J"),
            Tuple.Create(9, "K"),
            Tuple.Create(10, "L"),
            Tuple.Create(11, "M"),
            Tuple.Create(12, "N"),
            Tuple.Create(13, "P"),
            Tuple.Create(14, "R"),
            Tuple.Create(15, "S"),
            Tuple.Create(16, "T"),
            Tuple.Create(17, "U"),
            Tuple.Create(18, "V"),
            Tuple.Create(19, "W"),
            Tuple.Create(20, "X"),
            Tuple.Create(21, "Y"),
            Tuple.Create(22, "Z")
        };

        private readonly Regex _regex = new Regex(_regexString, RegexOptions.Compiled);

        public SSN_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.SSN_02)
        {
            _validLetters = _validLettersList.ToDictionary(x => x.Item1, x => x.Item2);
            _validLettersInverse = _validLettersList.ToDictionary(x => x.Item2, x => x.Item1);
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

            var ssn1To4 = ssn.Substring(0, 4);
            var ssn5To12 = ssn.Substring(4, 8).Select(c => Convert.ToInt32(c.ToString()));

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
            return _validLetters.ContainsKey(value) ? _validLetters[value] : null;
        }

        public int GetLetterValue(string letter)
        {
            return _validLettersInverse[letter];
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
