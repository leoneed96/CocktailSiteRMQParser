using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQTools.WorkingQueue
{
    public class WorkingQueueWrapper
    {
        private readonly RabbitMQConnection _rabbitMQConnection;
        private readonly ILogger<WorkingQueueWrapper> _logger;
        private readonly RabbitMQOptions _mQOptions;
        private bool _prepared;
        private IModel _model;
        private IBasicProperties _basicProperties;
        public WorkingQueueWrapper(RabbitMQConnection rabbitMQConnection, 
            ILogger<WorkingQueueWrapper> logger,
            IOptions<RabbitMQOptions> options)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _logger = logger;
            _mQOptions = options?.Value ?? throw new ArgumentNullException();
        }

        private async Task PrepareQueueAsync()
        {
            if (_prepared)
                return;
            if (!await _rabbitMQConnection.ConnectAsync(_mQOptions.Host, _mQOptions.Port))
                throw new InvalidOperationException();

            var model = _rabbitMQConnection.CreateModel(x =>
            {
                _basicProperties = x.CreateBasicProperties();
                // mark message as persistent - rmq saves it to disk
                _basicProperties.Persistent = true;
                // only one unacked message per worker. must be declared both in prod/cons
                x.BasicQos(0, 1, false);
            });

            model.QueueDeclare(_mQOptions.WorkingQueueName, durable: true, exclusive: false, autoDelete: false);
            _model = model;
            _prepared = true;
        }
        public void Dispose()
        {
            _model.Dispose();
            _rabbitMQConnection.Dispose();
        }
        public async Task PushToQueue<T>(T payload)
        {
            await PrepareQueueAsync();

            _model.BasicPublish(exchange: "",
                  routingKey: _mQOptions.WorkingQueueName,
                  basicProperties: _basicProperties,
                  body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
        }
        public async Task RegisterConsumer<T>(Func<T, Task<bool>> handler)
        {
            await PrepareQueueAsync();

            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += async (m, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                var payload = JsonConvert.DeserializeObject<T>(message);

                var handleResult = await handler(payload);

                if (handleResult)
                {
                    _logger.LogInformation("Consumer Tag#{0} executed successfully", e.ConsumerTag);
                    _model.BasicAck(e.DeliveryTag, false);
                }
                else
                {
                    _logger.LogError("Consumer Tag#{0} executed unsuccessfully", e.ConsumerTag);
                }
            };
            _model.BasicConsume(_mQOptions.WorkingQueueName, false, consumer);

        }

    }
}
