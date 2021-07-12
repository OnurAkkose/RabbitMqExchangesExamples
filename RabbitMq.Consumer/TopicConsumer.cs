using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMq.Consumer
{
    class TopicConsumer
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;
            var routeKey = "*.Error.*";
            channel.QueueBind(queueName, "logs-topic", routeKey);
            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("Logs are being listening");
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("The message => " + message);
                channel.BasicAck(e.DeliveryTag, false);
            };
            Console.ReadLine();
        }
    }
}
