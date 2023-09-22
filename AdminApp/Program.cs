using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;

namespace AdminApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "Dlx-exchange", type: ExchangeType.Direct, durable: true);
            channel.QueueDeclare("Dlx-queue", durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind("Dlx-queue", "Dlx-exchange", "tour");
            // durable = Durable exchanges survive broker restart whereas transient exchanges do not

            Console.WriteLine("Waiting for Admin messages. To exit press CTRL+C");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => //here we would handle the 'bad' message
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var routingKey = ea.RoutingKey;
                Console.WriteLine($" Log Received '{routingKey}':'{message}'");
            };
            channel.BasicConsume(queue: "Dlx-queue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.ReadLine();
        }
    }
}