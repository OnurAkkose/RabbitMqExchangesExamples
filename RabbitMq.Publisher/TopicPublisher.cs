using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{
    class TopicPublisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Topic);
            Random rnd = new Random();
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);
                var routeKey = $"{log1}.{log2}.{log3}";
                var message = $"log-type {log1}-{log2}-{log3}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", routeKey, null, messageBody);
                Console.WriteLine($"Message was send : {message}");
            });
            Console.ReadLine();
        }
    }
}
