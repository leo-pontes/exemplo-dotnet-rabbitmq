using AppOrderWorker.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace AppOrderWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orderQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {                    
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    var order = JsonSerializer.Deserialize<Order>(message);

                    Console.WriteLine($" Order Number {order.OrderNumber} | Name {order.ItemName} | Price {order.Price:N2}");                  
                };
                channel.BasicConsume(queue: "orderQueue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
