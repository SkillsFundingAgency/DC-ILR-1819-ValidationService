using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    /// <summary>
    /// the LARS standard data retrieval service
    /// </summary>
    public interface ILARSStandardDataRetrievalService : IExternalDataRetrievalService<List<LARSStandard>>
    {
    }
}
