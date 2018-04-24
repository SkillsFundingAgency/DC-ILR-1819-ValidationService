using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class Framework
    {
        public int FworkCode { get; set; }
        public int ProgType { get; set; }
        public int PwayCode { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public IEnumerable<FrameworkAim> FrameworkAims { get; set; }
        public IEnumerable<FrameworkCommonComponent> FrameworkCommonComponents { get; set; }
    }
}
