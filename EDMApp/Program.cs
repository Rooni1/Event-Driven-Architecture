using RabbitMQ.Client;
using System.Text;


ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Message Sender App";
IConnection conn = await factory.CreateConnectionAsync();
IChannel channel = await conn.CreateChannelAsync();
string exchangeName = "EDAExchange";
string routingKey = "EDA-routing-key";
string queueName = "EDAQueue";
channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct);
channel.QueueDeclareAsync(queueName,durable:false, exclusive:false,autoDelete:false,arguments:null);
channel.QueueBindAsync(queueName,exchangeName,routingKey,arguments:null);
for(int i = 0; i < 20; i++)
{
    Console.WriteLine(value:$"Sending Message{i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message #{i}: Hello Wellcome to EDA");
    channel.BasicPublishAsync(exchangeName, routingKey, messageBodyBytes);
    Thread.Sleep(1000);
}

channel.CloseAsync();
conn.CloseAsync();