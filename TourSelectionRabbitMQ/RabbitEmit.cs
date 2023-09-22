using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TourSelectionRabbitMQ
{
    public class RabbitEmit
    {
        public void Emit(Tour tour)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic,durable:true, autoDelete: false);
            // declare the queue where it should sent the dead letters (by the ttl (10 sec.))
            //made a exchange called Dlx-exchange with a queue called Dlx-queue (binded ,if type:direct use routing key, if type:fanout
            //takes all from exchange), see pictures on github repo.
            channel.QueueDeclare("topic_queue", durable: true, exclusive: false, autoDelete: false, 
                new Dictionary<string, object> { { "x-dead-letter-exchange", "Dlx-exchange"} , 
                    { "x-dead-letter-routing-key", "tour" }, { "x-message-ttl", 20000 } });

            var routingKey = "tour.";
            //bad code but does the job
            if (tour.Book)
            {
                routingKey = routingKey + "Book";
            }
            else if (tour.Cancel)
            {
                routingKey = routingKey + "Cancel";
            }
            else return;

            if (String.IsNullOrEmpty(tour.Name) && string.IsNullOrEmpty(tour.Email)) {
                tour = default; //set it to fail
            }
            
            var properties = channel.CreateBasicProperties();          

            var message = JsonSerializer.Serialize(tour);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic",
                                 routingKey: routingKey,
                                 basicProperties: properties,
                                 body: body);
   
  
        }
    }
}
