﻿using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    /// <summary>
    /// the postcodes data service implementation
    /// </summary>
    /// <seealso cref="IPostcodesDataService" />
    public class PostcodesDataService :
        IPostcodesDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        /// <summary>
        /// The ons postcodes
        /// </summary>
        private readonly IReadOnlyCollection<IONSPostcode> _onsPostcodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostcodesDataService"/> class.
        /// </summary>
        /// <param name="externalDataCache">The external data cache.</param>
        public PostcodesDataService(IExternalDataCache externalDataCache)
        {
            It.IsNull(externalDataCache)
                .AsGuard<ArgumentNullException>(nameof(externalDataCache));

            _externalDataCache = externalDataCache;
            _onsPostcodes = _externalDataCache.ONSPostcodes.AsSafeReadOnlyList();
        }

        public bool PostcodeExists(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode)
                   && _externalDataCache.Postcodes.Contains(postcode);
        }

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>an ons postcode (if found)</returns>
        public IONSPostcode GetONSPostcode(string fromPostcode) =>
            _onsPostcodes.FirstOrDefault(x => x.Postcode.ComparesWith(fromPostcode));

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>an ons postcodes (if found)</returns>
        public IReadOnlyCollection<IONSPostcode> GetONSPostcodes(string fromPostcode) =>
            _onsPostcodes.Where(x => x.Postcode.ComparesWith(fromPostcode)).ToList();
    }
}
