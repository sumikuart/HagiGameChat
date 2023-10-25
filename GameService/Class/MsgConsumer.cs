using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Class
{
    public static class MsgConsumer
    {


        public static void RabbitConnection(string guild, string userName, NetworkStream stream)
        {
            var Factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "Kim", Password = "Kim" };
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var exchange = "MsgExchanger";
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, autoDelete: false);
                channel.ExchangeDeclare(exchange: "MsgExchangerGlobal", type: ExchangeType.Fanout, autoDelete: false);
                var queueName = channel.QueueDeclare(
                    queue: userName,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null).QueueName;

                string[] routeKeys = $"{guild},{userName}".Split(',');
                foreach (var routeKey in routeKeys)
                {
                    channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routeKey);
                }
                channel.QueueBind(queue: queueName, exchange: "MsgExchangerGlobal", routingKey: "");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
                    stream.Write(msg, 0, msg.Length);
                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);
                while (true) 
                { 
                    Thread.Sleep(10000);
                }
            }

        }
    }
}
