using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext;
using ESFA.DC.JobContextManager.Interface;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ESFA.DC.ILR.ValidationService.Stateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Stateless : StatelessService
    {
        private readonly IJobContextManager<JobContextMessage> _jobContextManager;

        public Stateless(StatelessServiceContext context, IJobContextManager<JobContextMessage> jobContextManager)
            : base(context)
        {
            _jobContextManager = jobContextManager;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            bool initialised = false;
            try
            {
                await _jobContextManager.OpenAsync(cancellationToken);
                initialised = true;
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception ex)
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
            }
            finally
            {
                if (initialised)
                {
                    await _jobContextManager.CloseAsync(CancellationToken.None);
                }
            }
        }
    }
}
