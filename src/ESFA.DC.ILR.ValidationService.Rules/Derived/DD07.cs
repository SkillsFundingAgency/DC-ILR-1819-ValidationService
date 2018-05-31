using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DD07 : IDD07
    {
        private readonly IEnumerable<int?> _allowedProgTypes = new HashSet<int?>() { 2, 3, 20, 21, 22, 23, 25 };

        public bool IsApprenticeship(int? progType)
        {
            return _allowedProgTypes.Contains(progType);
        }
    }
}
