using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Text;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {
        public async Task BookTableRequest(string message)
        {
            string UserName = "guest";
            string Password = "guest";
            string HostName = "localhost";

            // Main entry point to the RabbitMQ .NET AMQP client
            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                UserName = UserName,
                Password = Password,
                HostName = HostName

            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();
            Console.WriteLine("Creating Exchange");

            // Create Exchange
            model.ExchangeDeclare("demoexchange", ExchangeType.Direct);

            // Create Queue
            model.QueueDeclare("demoqueue", true, false, false, null);
            Console.WriteLine("Creating Queue");

            // Bind Queue to Exchange
            model.QueueBind("demoqueue", "demoexchange", "directexchange_key");
            Console.WriteLine("Creating Binding");

            // -------------------------------
            // Publish a message to RabbitMQ Q
            // TableBooking service should consume this messge

            var properties = model.CreateBasicProperties();
            properties.Persistent = false;
            byte[] messagebuffer = Encoding.Default.GetBytes("Direct Message");
            model.BasicPublish("demoexchange", "directexchange_key", properties, messagebuffer);
            Console.WriteLine("Message Sent");

            // Listen to the messages on UITaskManager queue
            // When there is a message, check the response method
            // Call the response method
            string responseMethod = "BookTableResponse";
            string responseMessage = "Booking request received.";
            //await Clients.All.SendAsync(responseMethod, responseMessage);
            await Clients.Caller.SendAsync(responseMethod, responseMessage);
        }
    }
}