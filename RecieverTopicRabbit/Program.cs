using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverTopicRabbit;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);
// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;


string[] keys = {"tour.Cancel"}; // "tour.Cancel" or/and "tour.Book" or just giving one key without the foreach loop
foreach (var bindingKey in keys)
{
    channel.QueueBind(queue: queueName,
                      exchange: "topic",
                      routingKey: bindingKey);
}

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Tour messageJson = JsonSerializer.Deserialize<Tour>(message);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
};
channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();