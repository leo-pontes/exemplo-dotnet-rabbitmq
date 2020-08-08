using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProdutorMultiChannel
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                var queueName = "order";

                var channer1 = CreateChannel(connection);
                var channer2 = CreateChannel(connection);

                BuildPublishers(channer1, queueName, "Produtor A");
                BuildPublishers(channer2, queueName, "Produtor B");

                Console.ReadLine();
            }
        }

        public static IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }

        public static void BuildPublishers(IModel channel, string queue, string publisherName)
        {
            Task.Run(() =>
            {
                int count = 0;

                channel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                while (true)
                {
                    string message = $"OrderNumber: {count++}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", queue, null, body);

                    Console.WriteLine($"{publisherName} - [x] Sent {count}", message);
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
