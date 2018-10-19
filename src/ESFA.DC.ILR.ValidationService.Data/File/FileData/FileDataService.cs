using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData
{
    public class FileDataService :
        IFileDataService
    {
        private readonly IFileDataCache _fileDataCache;

        public FileDataService(IFileDataCache fileDataCache)
        {
            _fileDataCache = fileDataCache;
        }

        public int UKPRN()
        {
            return _fileDataCache.UKPRN;
        }

        public DateTime FilePreparationDate()
        {
            return _fileDataCache.FilePreparationDate;
        }

        public string FileName()
        {
            return _fileDataCache.FileName;
        }

        public int? FileNameUKPRN()
        {
            return _fileDataCache.FileNameUKPRN;
        }
    }
}
