namespace RabbitMQTools
{
    public class RabbitMQOptions
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string WorkingQueueName { get; set; }
        public int MaxQueueLength { get; set; } = 5;
    }
}
