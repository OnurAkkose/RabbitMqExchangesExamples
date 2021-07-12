using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{
    class FanoutPublisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("logs-fanout",durable:true, type:ExchangeType.Fanout);
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                var message = $"log {x}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", "", null, messageBody);
                Console.WriteLine($"Message was send : {message}");
            });
        }
    }
   
}
