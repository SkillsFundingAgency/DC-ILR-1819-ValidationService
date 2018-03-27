using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DD07 : IDD07
    {
        private readonly IEnumerable<long?> _allowedProgTypes = new HashSet<long?>() { 2, 3, 20, 21, 22, 23, 25 };

        public string Derive(long? progType)
        {
            return _allowedProgTypes.Contains(progType) ? ValidationConstants.Y : ValidationConstants.N;
        }
    }
}
