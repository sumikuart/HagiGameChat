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

        public static void CreateRabbitConnection()
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "Kim", Password = "Kim" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            activeChannel = channel;

            var consumer = new EventingBasicConsumer(activeChannel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received: {0}", message);
                static_message = message;
            };
            activeChannel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
        }


    }
}
