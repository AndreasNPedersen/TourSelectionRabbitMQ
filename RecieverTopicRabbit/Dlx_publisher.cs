using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecieverTopicRabbit
{
    public class Dlx_publisher
    {
        private IModel _channel;
        //Could also be correctly called invalid message publisher
        public Dlx_publisher(IModel channel) { 
         _channel = channel;
        }

        public void PublishToLog(string message, string routingKey)
        {
            // this could have be to an invalid message channel

            _channel.ExchangeDeclare(exchange: "Dlx-exchange", type: ExchangeType.Direct, durable: true, autoDelete: false);

            _channel.QueueDeclare("Dlx-queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("Dlx-queue", "Dlx-exchange", "tour"); 
            // to dead letter exchange binding
            var properties = _channel.CreateBasicProperties();

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "Dlx-exchange",
                             routingKey: routingKey.Substring(0,4),
                             basicProperties: properties,
                             body: body);


        }
    }
}
