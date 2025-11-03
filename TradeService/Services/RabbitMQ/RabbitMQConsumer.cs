using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using TradeService.Config;
using System.Text;
using TradeService.Services.HttpService;
using Google.Protobuf;
using InventorySerivce.Models.Protobufs;

namespace TradeService.Services.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ITradeRequestHttpService _TradeRequestService;
        private readonly Dictionary<string, Func<byte[], IMessage>> _deserializationMap;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> options, TradeRequestHttpService tradeRequestHttpService)
        {
            _TradeRequestService = tradeRequestHttpService;

            var factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _deserializationMap = new Dictionary<string, Func<byte[], IMessage>>
        {
            { "trade-sell-queue", body => SendTradeDto.Parser.ParseFrom(body) },
            //{ "trade-buy-queue", body => TradeBuyDto.Parser.ParseFrom(body) }
        };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var queueName in _deserializationMap.Keys)
            {
                _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, content) => HandleMessage(content);
                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }

            return Task.CompletedTask;
        }

        private void HandleMessage(BasicDeliverEventArgs content)
        {
            var queueName = content.RoutingKey;
            var body = content.Body.ToArray();

            if (_deserializationMap.TryGetValue(queueName, out var deserializeFunction))
            {
                var message = deserializeFunction(body);
                ProcessMessage(message);
            }
            else
            {
                Console.WriteLine($"No handler for queue {queueName}");
            }
        }

        private void ProcessMessage(IMessage message)
        {
            switch (message)
            {
                case SendTradeDto sendTradeDto:
                    ProcessTradeSellMessage(sendTradeDto);
                    break;
                default:
                    Console.WriteLine("Unknown message type");
                    break;
            }
        }
        private void ProcessTradeSellMessage(SendTradeDto sendTradeDto)
        {
            Console.WriteLine($"Processing TradeSellDto: {sendTradeDto.TradeId}");
            _TradeRequestService.SendTradeRequestAsync(sendTradeDto).Wait();
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
