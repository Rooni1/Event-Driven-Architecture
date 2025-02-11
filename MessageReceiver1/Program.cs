using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "EDA Receiver1";
IConnection conn = await factory.CreateConnectionAsync();
IChannel channel = await conn.CreateChannelAsync();
string exchangename = "EDAExchange";
string routingKey = "EDA-routing-key";
string queueName = "EDAQueue";
channel.ExchangeDeclareAsync(exchangename, ExchangeType.Direct);
channel.QueueDeclareAsync(queueName,durable: false,exclusive:false,autoDelete:false,arguments:null);
channel.QueueBindAsync(queueName,exchangename,routingKey,arguments:null);
channel.BasicQosAsync(prefetchSize:0,prefetchCount:1,global:false);
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, args) =>
{
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine(value:$"Message Received:{ message}");
    channel.BasicAckAsync(args.DeliveryTag, multiple: false);
};
string consumerTag = channel.BasicConsumeAsync(queueName, autoAck: false, consumer).ToString();
Console.ReadLine();
channel.BasicCancelAsync(consumerTag);
channel.CloseAsync();
conn.CloseAsync();

