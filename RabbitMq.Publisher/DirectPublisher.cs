using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    class DirectPublisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x => 
            {
                var routeKey = $"route-{x}";
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName, true, false);
                channel.QueueBind(queueName, "logs-direct", routeKey,null);

            });
            
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log =(LogNames)new Random().Next(1, 4);                
                var message = $"log-type {log}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                var routeKey = $"route-{log}";
                channel.BasicPublish("logs-fanout", routeKey, null, messageBody);
                Console.WriteLine($"Message was send : {message}");
            });
            Console.ReadLine();
        }

    }
}
