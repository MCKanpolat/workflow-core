using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.Util;
using WorkflowCore.Interface;

namespace WorkflowCore.QueueProviders.ActiveMQ.Services
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class ActiveMQProvider : IQueueProvider
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private ISession _session;
        protected IDestination _eventDestination;
        protected IDestination _workflowDestination;
        private readonly string _workflowQueueName;
        private readonly string _eventQueueName;

        public bool IsDequeueBlocking => false;

        public ActiveMQProvider(IConnectionFactory connectionFactory, string workflowQueueName, string eventQueueName)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _workflowQueueName = workflowQueueName;
            _eventQueueName = eventQueueName;
        }

        public async Task QueueWork(string id, QueueType queue)
        {
            if (_connection == null)
                throw new InvalidOperationException("ActiveMQ provider not running");

            var destination = GetDestination(queue);
            if (destination == null)
                throw new InvalidOperationException("Invalid destination");

            var msg = _session.CreateTextMessage(id);
            using (var producer = _session.CreateProducer(destination))
            {
                producer.Send(msg);
            }
        }

        public async Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken)
        {
            if (_connection == null)
                throw new InvalidOperationException("ActiveMQ provider not running");

            var destination = GetDestination(queue);

            if (destination == null)
                throw new InvalidOperationException("Invalid destination");

            string result = null;
            using (var consumer = _session.CreateConsumer(destination))
            {
                var message = consumer.ReceiveNoWait();
                if (message != null)
                {
                    var txtMsg = message as ITextMessage;
                    if (txtMsg != null)
                    {
                        result = txtMsg.Text;
                    }
                    message.Acknowledge();
                }
            }
            return result;
        }

        public void Dispose()
        {
            Stop();
        }

        public async Task Start()
        {
            _connection = _connectionFactory.CreateConnection();
            _connection.Start();

            _session = _connection.CreateSession(AcknowledgementMode.ClientAcknowledge);
            _eventDestination = SessionUtil.GetDestination(_session, GetQueueName(QueueType.Event), DestinationType.Queue);
            _workflowDestination = SessionUtil.GetDestination(_session, GetQueueName(QueueType.Workflow), DestinationType.Queue);
        }

        public async Task Stop()
        {
            if (_connection != null)
            {
                _session.Close();
                _session = null;

                _connection.Close();
                _connection = null;
            }
        }

        private string GetQueueName(QueueType queue)
        {
            switch (queue)
            {
                case QueueType.Workflow:
                    return _workflowQueueName;
                case QueueType.Event:
                    return _eventQueueName;
            }
            return null;
        }

        private IDestination GetDestination(QueueType queue)
        {
            switch (queue)
            {
                case QueueType.Workflow:
                    return _workflowDestination;
                case QueueType.Event:
                    return _eventDestination;
            }
            return null;
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
