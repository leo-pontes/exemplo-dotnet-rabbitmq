using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsumidorMultiChannel
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                BuildAndRunWorker(channel, "Consumidor 1");
                BuildAndRunWorker(channel, "Consumidor 2");
                BuildAndRunWorker(channel, "Consumidor 3");

                Console.ReadLine();
            }
        }

        public static void BuildAndRunWorker(IModel channel, string workerName)
        {
            channel.BasicQos(0, 5, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"{workerName}: [x] Received {0}", message);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }

            };
            channel.BasicConsume(queue: "order",
                                 autoAck: false,
                                 consumer: consumer);

            
        }
    }
}
