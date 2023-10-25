using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SessionService.Class
{
    public static class ConnectionManager
    {
        public static string routerKey = "MsgInfo";
        public static string exchange = "MsgExchanger";
        public static string static_message;
        public static IModel activeChannel;


        public static void ProduceMsg(string msg)
        {
            var Factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                Thread.Sleep(2000);

                var exchange = "MsgExchanger";
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

              
                    SendMessage(msg, channel, exchange, "MsgInfo", true);
                    Thread.Sleep(500);

                }



                Console.WriteLine("All Message Sends");
                Console.ReadLine();
         }

        public static void SendMessage(string msg, IModel channel, string exchange, string routeingKey, bool expectReplay)
        {

            var body = Encoding.UTF8.GetBytes(msg);

            IBasicProperties? properties = null;
            string? replayQueue = channel.QueueDeclare().QueueName;
            string? corID = Guid.NewGuid().ToString();

            if (expectReplay)
            {
                properties = channel.CreateBasicProperties();
                corID = Guid.NewGuid().ToString();
                replayQueue = channel.QueueDeclare().QueueName;
                properties.CorrelationId = corID;
                properties.ReplyTo = replayQueue;
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == corID)
                    {
                        Console.WriteLine($"Response MS {response}");
                    }
                };
                channel.BasicConsume(
                    consumer: consumer,
                    queue: replayQueue,
                    autoAck: true);


            }

            channel.BasicPublish(exchange: exchange,
            routingKey: routeingKey,
            basicProperties: properties,
            body: body);

            Console.WriteLine("msg Send: " + msg);
        }

    }
}
