using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace SessionService.Class
{
    public static class ConnectionManager
    {
  
        public static string exchange = "MsgExchanger";
        public static string static_message;
        public static IModel activeChannel;


      

        public static void ProducePrivateMsg(string msg, string target)
        {
            var Factory = new ConnectionFactory() { HostName = "rabbitmq", UserName="Kim", Password="Kim" };
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                   string routingKey = "*." + target;        
                   var exchange = "MsgExchanger";
                   channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);         
                   SendMessage(msg, channel, exchange, routingKey);
                    

             }
         }

        public static void ProduceGuildMsg(string msg, string target)
        {
            var Factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "Kim", Password = "Kim" };
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                string routingKey = target + ".*";
                var exchange = "MsgExchanger";
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic);


                SendMessage(msg, channel, exchange, routingKey);
               

            }

        }

        public static void SendMessage(string msg, IModel channel, string exchange, string routeingKey)
        {

            var body = Encoding.UTF8.GetBytes(msg);

            IBasicProperties? properties = null;

            channel.BasicPublish(exchange: exchange,
            routingKey: routeingKey,
            basicProperties: properties,
            body: body);

           
        }

    }
}
