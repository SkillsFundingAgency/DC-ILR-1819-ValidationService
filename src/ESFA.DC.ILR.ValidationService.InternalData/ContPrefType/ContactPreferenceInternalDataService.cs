using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ESFA.DC.ILR.ValidationService.InternalData.ContPrefType.Models;

namespace ESFA.DC.ILR.ValidationService.InternalData.ContPrefType
{
    public class ContactPreferenceInternalDataService : IContactPreferenceInternalDataService
    {
        private readonly IReadOnlyCollection<ContactPreferenceInternalData> _validContactPreferenceTypeCodesLookup;

        public ContactPreferenceInternalDataService()
        {
            _validContactPreferenceTypeCodesLookup = new ReadOnlyCollection<ContactPreferenceInternalData>(new List<ContactPreferenceInternalData>()
            {
                new ContactPreferenceInternalData()
                {
                    Type = "PMC",
                    Code = 1,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "PMC",
                    Code = 2,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "PMC",
                    Code = 3,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "RUI",
                    Code = 1,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "RUI",
                    Code = 2,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "RUI",
                    Code = 3,
                    ValidTo = new DateTime(2013, 07, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "RUI",
                    Code = 4,
                    ValidTo = new DateTime(2099, 12, 31)
                },
                new ContactPreferenceInternalData()
                {
                    Type = "RUI",
                    Code = 5,
                    ValidTo = new DateTime(2099, 12, 31)
                }
            });
        }

        public bool TypeExists(string type)
        {
            return _validContactPreferenceTypeCodesLookup.Any(x => x.Type == type);
        }

        public bool CodeExists(long? code)
        {
            if (!code.HasValue)
            {
                return false;
            }

            return _validContactPreferenceTypeCodesLookup.Any(x => x.Code == code.Value);
        }

        public bool TypeForCodeExist(string type, long? code, DateTime? validTo)
        {
            if (string.IsNullOrWhiteSpace(type) || !code.HasValue || !validTo.HasValue)
            {
                return false;
            }

            return _validContactPreferenceTypeCodesLookup.Any(x => x.Type == type && x.Code == code.Value && validTo <= x.ValidTo);
        }
    }
}