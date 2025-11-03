namespace TradeService.Services.RabbitMQ
{
    public interface IRabbitMQProducer : IDisposable
    {
        void SendMessage(string message, string queueName);
    }
}
