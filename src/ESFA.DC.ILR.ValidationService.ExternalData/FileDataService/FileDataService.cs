using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;

namespace ESFA.DC.ILR.ValidationService.ExternalData.FileDataService
{
    public class FileDataService : IFileDataService
    {
        public DateTime FilePreparationDate { get; private set; }

        public void Populate(IMessage message)
        {
            FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate;
        }
    }
}
