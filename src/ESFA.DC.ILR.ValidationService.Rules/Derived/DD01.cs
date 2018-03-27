﻿using System.Linq;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Extensions;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DD01 : IDD01
    {
        public string Derive(long? uln)
        {
            if (uln == ValidationConstants.TemporaryULN)
            {
                return ValidationConstants.Y;
            }

            if (!uln.HasValue || uln.ToString().Length != 10)
            {
                return ValidationConstants.N;
            }

            var checkSum = CalculateCheckSum(uln.Value);

            if (checkSum == 0)
            {
                return ValidationConstants.N;
            }

            return (10 - checkSum).ToString();
        }

        public int CalculateCheckSum(long uln)
        {
            int[] array = uln.ToDigitEnumerable().ToArray();

            int total = 0;

            for (int i = 0; i < 9; i++)
            {
                total += array[i] * (10 - i);
            }

            return total % 11;
        }
    }
}
