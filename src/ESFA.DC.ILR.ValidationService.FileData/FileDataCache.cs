using System;
using ESFA.DC.ILR.ValidationService.Rules.File.Interface;

namespace ESFA.DC.ILR.ValidationService.FileData
{
    public class FileDataCache : IFileDataCache
    {
        public DateTime FilePreparationDate { get; set; }
    }
}
