using System;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.File
{
    public class FileDataCache : IFileDataCache
    {
        public DateTime FilePreparationDate { get; set; }
    }
}
