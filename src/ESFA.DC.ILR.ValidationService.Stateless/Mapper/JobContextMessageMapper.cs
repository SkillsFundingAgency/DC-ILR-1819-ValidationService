using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.JobContext;
using ESFA.DC.Mapping.Interface;

namespace ESFA.DC.ILR.ValidationService.Stateless.Mapper
{
    public class JobContextMessageMapper : IMapper<JobContextMessage, JobContextMessage>
    {
        public JobContextMessage MapTo(JobContextMessage value)
        {
            return value;
        }

        public JobContextMessage MapFrom(JobContextMessage value)
        {
            return value;
        }
    }
}
