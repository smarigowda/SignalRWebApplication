using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using SignalRWebApplication.Types;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {
        protected readonly IModel channel;
        protected string queueName;

        public TableBookingHub(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
            channel = channelService.getChannel();
            queueName = configuration.GetValue<string>("RabbitMQNames:SignalRWeb");
        }

        public void BookTableRequest(string message)
        {

            // store the request in MongoDB
            // code here...


            // Send response to client
            string exchangeName = "demoexchange";
            string keyBindingName = "directexchange_key";
            string responseMessage = "Table booking request received. Thanks for using our online booking service.";

            MessageType messageObject;
            messageObject = JsonSerializer.Deserialize<MessageType>(message);

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