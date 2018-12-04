using System;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class StringExtensions
    {
        public static bool CaseInsensitiveEquals(this string source, string data)
        {
            return source != null ? source.Equals(data, StringComparison.OrdinalIgnoreCase) : false;
        }
    }
}