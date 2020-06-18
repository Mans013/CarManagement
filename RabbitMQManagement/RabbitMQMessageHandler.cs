using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using System.Collections.Generic;

namespace RabbitMQManagement
{
    public class RabbitMQMessageHandler
    {
        private readonly string _host;
        private readonly string _exchange;
        private readonly string _queuename;
        private readonly List<string> _routingKeys;
        private IConnection _connection;
        private IModel _model;
        private AsyncEventingBasicConsumer _consumer;
        private string _consumerTag;
        private IMessageHandlerCallback _callback;

        public RabbitMQMessageHandler(string host, string exchange, string queuename, List<string> routingKeys)
        {
            _host = host;
            _exchange = exchange;
            _queuename = queuename;
            _routingKeys = routingKeys;
        }

        public void Start(IMessageHandlerCallback callback)
        {
            _callback = callback;

            Policy
                .Handle<Exception>()
                .WaitAndRetry(9, r => TimeSpan.FromSeconds(5), (ex, ts) => {  })
                .Execute(() =>
                {
                    var factory = new ConnectionFactory() { HostName = _host, DispatchConsumersAsync = true };
                    _connection = factory.CreateConnection();
                    _model = _connection.CreateModel();
                    _model.ExchangeDeclare(_exchange, "topic", durable: true, autoDelete: false);
                    _model.QueueDeclare(_queuename, durable: true, autoDelete: false, exclusive: false);

                    _routingKeys.ForEach(routingkey =>
                    {
                        _model.QueueBind(_queuename, _exchange, routingKey: routingkey);
                    });

                    _consumer = new AsyncEventingBasicConsumer(_model);
                    _consumer.Received += Consumer_Received;
                    _consumerTag = _model.BasicConsume(_queuename, false, _consumer);
                });
        }

        public void Stop()
        {
            _model.BasicCancel(_consumerTag);
            _model.Close(200, "Goodbye");
            _connection.Close();
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            if (await HandleEvent(ea))
            {
                _model.BasicAck(ea.DeliveryTag, false);
            }
        }

        private Task<bool> HandleEvent(BasicDeliverEventArgs ea)
        {
            string messageType = Encoding.UTF8.GetString((byte[])ea.BasicProperties.Headers["MessageType"]);
            string body = Encoding.UTF8.GetString(ea.Body.ToArray());
            return _callback.HandleMessageAsync(messageType, body);
        }
    }
}
