using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMq.Consumer
{
    class DirectConsumer
    {
        static void Direct(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal", UserName = "guest", Password = "guest" };
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var randomQueueName = channel.QueueDeclare().QueueName;
            channel.QueueDeclare(randomQueueName, true, false, false);
            channel.QueueBind(randomQueueName, "logs-direct", "", null);
            channel.BasicQos(0, 1, false);

            var queueName = "direct-queue-Critical";
            var consumer = new EventingBasicConsumer(channel);
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
