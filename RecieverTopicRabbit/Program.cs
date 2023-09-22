using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverTopicRabbit;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
Dlx_publisher dlx_Publisher = new Dlx_publisher(channel); //initialize the invalid message channel 

channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic, durable: true);

channel.QueueDeclare("topic_queue", durable: true, exclusive: false, autoDelete: false,
                new Dictionary<string, object> { { "x-dead-letter-exchange", "Dlx-exchange" },
                    { "x-dead-letter-routing-key", "tour"}, { "x-message-ttl", 20000 } });
// durable = Durable exchanges survive broker restart whereas transient exchanges do not

string[] keys = { "tour.Cancel", "tour.Book" }; // "tour.Cancel" or/and "tour.Book" or just giving one key without the foreach loop
foreach (var bindingKey in keys)
{
    channel.QueueBind(queue: "topic_queue",
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
    if (messageJson == null)
    {
        dlx_Publisher.PublishToLog(message, routingKey); //invalid message
        return;
    }
    Console.WriteLine($"routing key:{routingKey} with message: {message}");

};
channel.BasicConsume(queue: "topic_queue",
                     autoAck: true,
                     consumer: consumer);


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();