using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    /// <summary>
    /// the postcodes service definition
    /// </summary>
    public interface IPostcodesDataService
    {
        /// <summary>
        /// Postcode exists.
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns>returns true if the postcode is found</returns>
        bool PostcodeExists(string postcode);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>an ons postcode (if found)</returns>
        IONSPostcode GetONSPostcode(string fromPostcode);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="fromPostcode">From postcode.</param>
        /// <returns>an ons postcodes (if found)</returns>
        IReadOnlyCollection<IONSPostcode> GetONSPostcodes(string fromPostcode);
    }
}
