using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface
{
    public interface IFileDataService
    {
        int UKPRN();

        DateTime FilePreparationDate();

        string FileName();

        int? FileNameUKPRN();
    }
}
