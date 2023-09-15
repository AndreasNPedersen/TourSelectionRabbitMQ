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

            channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);
            
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
            

            var message = JsonSerializer.Serialize(tour);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
