using ESFA.DC.ILR.Model.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface
{
    public interface IFileDataService
    {
        DateTime FilePreparationDate { get; }

        void Populate(IMessage message);
    }
}
