using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Class
{
    internal class MsgConsumer
    {


        public void RabbitConnection(string guild, string userName)
        {



            var Factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "Kim", Password = "Kim" };
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var exchange = "MsgExchanger";
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);
                var queueName = channel.QueueDeclare().QueueName;

                string routeKeys = $"{guild}.{userName}";
                channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routeKeys);


                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {


                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);



                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);


            }

        }
    }
}
