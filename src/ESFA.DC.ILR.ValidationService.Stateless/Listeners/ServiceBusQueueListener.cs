using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Stateless.Models;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace ESFA.DC.ILR.ValidationService.Stateless.Listeners
{
    public class ServiceBusQueueListener : ICommunicationListener
    {
        private Func<ServiceBusQueueListenerModel, Task> _callback;
        private string _serviceBusConnectionString;
        private QueueClient _queueClient;
        private ILogger _logger;

        public ServiceBusQueueListener(Func<ServiceBusQueueListenerModel, Task> callback, string serviceBusConnectionString, string queueName, ILogger logger)
        {
            _callback = callback;
            _serviceBusConnectionString = serviceBusConnectionString;
            var retryPolicy = new RetryExponential(TimeSpan.Zero, TimeSpan.FromMinutes(5), 3);

            _queueClient = new QueueClient(serviceBusConnectionString, queueName, ReceiveMode.PeekLock, retryPolicy);
            _logger = logger;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var sessionHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // MaxConcurrentSessions  = Environment.ProcessorCount
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to no. of processor count.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = Environment.ProcessorCount,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(6)
            };

            // Register the function that processes messages.
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, sessionHandlerOptions);

            // Return the uri - in this case, that's just our connection string
            return Task.FromResult(_serviceBusConnectionString);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.FromResult(true);
        }

        public void Abort()
        {
            Stop();
        }

        private void Stop()
        {
            if (_queueClient != null && !_queueClient.IsClosedOrClosing)
            {
                _queueClient.CloseAsync();
                _queueClient = null;
            }
        }

        async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            _logger.LogDebug($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Process the message.
            // call handler
            await _callback(new ServiceBusQueueListenerModel()
            {
                Message = message,
                Token = token
            });

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        // Use this handler to examine the exceptions received on the message pump.
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            sb.AppendLine("Exception context for troubleshooting:");
            sb.AppendLine($"- Endpoint: {context.Endpoint}");
            sb.AppendLine($"- Entity Path: {context.EntityPath}");
            sb.AppendLine($"- Executing Action: {context.Action}");
            _logger.LogError(sb.ToString(), exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
