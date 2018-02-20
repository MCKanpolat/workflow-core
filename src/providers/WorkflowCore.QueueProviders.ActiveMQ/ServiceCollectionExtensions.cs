using Apache.NMS;
using WorkflowCore.Models;
using WorkflowCore.QueueProviders.ActiveMQ.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseActiveMQ(this WorkflowOptions options, IConnectionFactory connectionFactory, string workflowQueueName = "wfc.workflow_queue",
            string eventQueueName = "wfc.event_queue")
        {
            options.UseQueueProvider(sp => new ActiveMQProvider(connectionFactory, workflowQueueName, eventQueueName));
            return options;
        }
    }
}
