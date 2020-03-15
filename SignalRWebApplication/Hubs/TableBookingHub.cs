using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using SignalRWebApplication.Types;
using Microsoft.Extensions.Configuration;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {
        protected readonly IModel channel;
        protected string queueName;
        protected IMongoClient dbClient;

        public TableBookingHub(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
            var mongoDBService = (IMongoDBService)serviceProvider.GetService(typeof(IMongoDBService));
            dbClient = mongoDBService.getClient();
            channel = channelService.getChannel();
            queueName = configuration.GetValue<string>("RabbitMQNames:SignalRWeb");
        }

        public void BookTableRequest(string message)
        {
            MessageType messageObject;
            messageObject = JsonSerializer.Deserialize<MessageType>(message);
            string text = messageObject.text;
            string email = messageObject.email;
            string name = messageObject.name;
            string phone = messageObject.phone;
            string day = messageObject.day;
            string month = messageObject.month;
            string year = messageObject.year;
            string startTime = messageObject.startTime;
            string endTime = messageObject.endTime;

            // store the request in MongoDB
            // code here...
            IMongoDatabase db = dbClient.GetDatabase("booking");
            var table = db.GetCollection<BsonDocument>("table");
            var doc = new BsonDocument
            {
                {"text", text},
                {"email", email},
                {"name", name},
                {"phone", phone},
                {"day", day},
                {"month", month},
                {"year", year},
                {"startTime", startTime},
                {"endTime", endTime}
            };
            table.InsertOne(doc);

            // Send response to client
            string exchangeName = "demoexchange";
            string keyBindingName = "directexchange_key";
            string responseMessage = "Table booking request received. Thanks for using our online booking service.";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queueName, exchangeName, keyBindingName);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = false;

            byte[] messagebuffer = Encoding.Default.GetBytes(responseMessage);
            channel.BasicPublish(exchangeName, keyBindingName, properties, messagebuffer);
        }
    }
}