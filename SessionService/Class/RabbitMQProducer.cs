using RabbitMQ.Client;
using SessionService.Interface;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace SessionService.Class
{
    public class RabbitMQProducer: IMessageProducer
    {
       
             bool expectReplay = false;

            public void SendMessage<T>(T message)
            {
         

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            IBasicProperties? properties = null;
            string? replayQueue = ConnectionManager.activeChannel.QueueDeclare().QueueName;
            string? corID = Guid.NewGuid().ToString();

            if (expectReplay)
            {
                properties = ConnectionManager.activeChannel.CreateBasicProperties();
                corID = Guid.NewGuid().ToString();
                replayQueue = ConnectionManager.activeChannel.QueueDeclare().QueueName;
                properties.CorrelationId = corID;
                properties.ReplyTo = replayQueue;
                var consumer = new EventingBasicConsumer(ConnectionManager.activeChannel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == corID)
                    {
                        Console.WriteLine($"Response MS {response}");
                    }
                };
                ConnectionManager.activeChannel.BasicConsume(
                    consumer: consumer,
                    queue: replayQueue,
                    autoAck: true);


            }

            ConnectionManager.activeChannel.BasicPublish(exchange: ConnectionManager.exchange,
            routingKey: ConnectionManager.routerKey,
            basicProperties: properties,
            body: body);

        }

    
    }
}
