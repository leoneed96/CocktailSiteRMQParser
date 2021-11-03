using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RabbitMQTools
{
    /// <summary>
    /// Register as singletone
    /// </summary>
    public class RabbitMQConnection : IDisposable
    {
        private IConnection _connection;
        private readonly ILogger<RabbitMQConnection> _logger;

        public RabbitMQConnection(ILogger<RabbitMQConnection> logger)
        {
            _logger = logger;
        }

        private bool _connected;
        private bool _disposed;
        public async Task<bool> ConnectAsync(string host, int port, int retryCount = 3)
        {
            int tryCounter = 0;
            while (!_connected && tryCounter < retryCount)
            {
                try
                {
                    _connection = new ConnectionFactory()
                    {
                        HostName = host,
                        Port = port
                    }.CreateConnection();

                    _connected = true;
                }
                catch (Exception e)
                {
                    _logger.LogWarning("RabbitMQ connect attempt #{0} failed. {1}", tryCounter + 1, e.Message);
                    tryCounter++;
                    await Task.Delay((int)(Math.Pow(tryCounter, 2) * 1000));
                }
            }
            if (!_connected)
            {
                _logger.LogError($"Can't estabilish RabbitMQ connection after {retryCount} attemts");
            }

            return _connected;
        }
        public IModel CreateModel(Action<IModel> configure = null)
        {
            var model = _connection.CreateModel();
            if (configure != null)
                configure(model);
            return model;
        }
        public void Dispose()
        {
            if (!_disposed)
                _connection.Dispose();
            _disposed = true;
        }
    }
}
