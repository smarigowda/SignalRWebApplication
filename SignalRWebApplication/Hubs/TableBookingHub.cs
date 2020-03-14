using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Text.Json;
using SignalRWebApplication.Types;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {
        protected readonly IModel channel;

        public TableBookingHub(IServiceProvider serviceProvider)
        {
            var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
            channel = channelService.getChannel();
        }

        public async Task BookTableRequest(string message)
        {
            string UserName = "guest";
            string Password = "guest";
            string HostName = "localhost";

            string exchangeName = "demoexchange";
            string keyBindingName = "directexchange_key";

            MessageType messageObject;
            messageObject = JsonSerializer.Deserialize<MessageType>(message);
        }
    }
}