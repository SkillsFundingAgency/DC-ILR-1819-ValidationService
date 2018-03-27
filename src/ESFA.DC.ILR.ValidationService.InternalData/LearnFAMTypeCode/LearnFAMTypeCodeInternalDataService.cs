using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode
{
    public class LearnFAMTypeCodeInternalDataService : ILearnFAMTypeCodeInternalDataService
    {
        private readonly IReadOnlyCollection<LearnFAMTypeCodeInternalData> _learnFamTypeCodesLookup;

        public LearnFAMTypeCodeInternalDataService()
        {
            var validTo = new DateTime(2099, 12, 31);

            _learnFamTypeCodesLookup = new ReadOnlyCollection<LearnFAMTypeCodeInternalData>(new List<LearnFAMTypeCodeInternalData>()
            {
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="HNS",Code = 1, ValidTo = validTo
                },

                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="EHC",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="DLA",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 36, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 55, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 56, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 57, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 58, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 59, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 60, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="LSR",Code = 61, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="NLM",Code = 17, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="NLM",Code = 18, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="FME",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="FME",Code = 2, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="PPE",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="PPE",Code = 2, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="SEN",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="EDF",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="EDF",Code = 2, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="MCF",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="MCF",Code = 2, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="MCF",Code = 3, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="MCF",Code = 4, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="ECF",Code = 1, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="ECF",Code = 2, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="ECF",Code = 3, ValidTo = validTo
                },
                new  LearnFAMTypeCodeInternalData()
                {
                    Type ="ECF",Code = 4, ValidTo = validTo
                }
            });
        }

        public bool TypeExists(string type)
        {
            return _learnFamTypeCodesLookup.Any(x => x.Type == type);
        }

        public bool TypeCodeExists(string type, long? code)
        {
            if (string.IsNullOrWhiteSpace(type) || !code.HasValue)
            {
                return false;
            }

            return _learnFamTypeCodesLookup.Any(x => x.Type == type && x.Code == code.Value);
        }

        public bool TypeCodeForDateExists(string type, long? code, DateTime? validTo)
        {
            if (string.IsNullOrWhiteSpace(type) || !code.HasValue || !validTo.HasValue)
            {
                return false;
            }

            return _learnFamTypeCodesLookup.Any(x => x.Type == type && x.Code == code.Value && validTo <= x.ValidTo);
        }
    }
}