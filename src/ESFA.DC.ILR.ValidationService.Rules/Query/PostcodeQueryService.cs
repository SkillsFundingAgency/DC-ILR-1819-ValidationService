using System.Text.RegularExpressions;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class PostcodeQueryService : IPostcodeQueryService
    {
        private const string _regexString = @"^[A-Z]{1,2}[0-9][0-9A-Z]?\s[0-9][ABD-HJLNP-UWXYZ]{2}$";

        private readonly Regex _regex = new Regex(_regexString, RegexOptions.Compiled);

        public bool RegexValid(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode) && _regex.IsMatch(postcode);
        }
    }
}