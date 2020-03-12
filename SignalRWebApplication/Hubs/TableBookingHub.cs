using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using SignalRWebApplication.Types;
using RabbitMQ.Client.Events;
//using RabbitMQConsumer;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {

        public TableBookingHub()
        {
            //_hubContext = hubContext;

            string exchangeName = "demoexchange";
            string keyBindingName = "directexchange_key";

            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            const string queueName = "demoqueue";
            //string exchangeName = "demoexchange";
            //string keyBindingName = "directexchange_key";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, keyBindingName);

            // consumer
            //var consumer = new AsyncEventingBasicConsumer(channel);
            //consumer.Received += Consumer_Received;
            //channel.BasicConsume(queueName, true, consumer);
            //Console.WriteLine("Creating Exchange");
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

            var factory = new ConnectionFactory() { DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // Main entry point to the RabbitMQ .NET AMQP client
            //var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            //{
            //    UserName = UserName,
            //    Password = Password,
            //    HostName = HostName

            //};

            //var connection = connectionFactory.CreateConnection();
            //var model = connection.CreateModel();

            // Create Exchange
            //model.ExchangeDeclare("demoexchange", ExchangeType.Direct);

            // Create Queue
            //model.QueueDeclare("demoqueue", true, false, false, null);
            //Console.WriteLine("Creating Queue");

            // Bind Queue to Exchange
            //model.QueueBind("demoqueue", "demoexchange", "directexchange_key");
            //Console.WriteLine("Creating Binding");

            // Create Queue
            //model.QueueDeclare("CloadTest.UserInterfaceTaskManager", true, false, false, null);
            //Console.WriteLine("Creating Queue");
            // Bind Queue to Exchange
            //model.QueueBind("CloadTest.UserInterfaceTaskManager", "demoexchange", "uitm");
            //Console.WriteLine("Creating Binding");

            // -------------------------------
            // Publish a message to RabbitMQ Q
            // TableBooking service should consume this messge
            var properties = channel.CreateBasicProperties();
            properties.Persistent = false;
            byte[] messagebuffer = Encoding.Default.GetBytes(message);
            channel.BasicPublish(exchangeName, keyBindingName, properties, messagebuffer);
            Console.WriteLine("Message Sent");

            // Listen to the messages on CloadTest.UserInterfaceTaskManager queue
            // When there is a message, check the response method// accept only one unack-ed message at a time

            //model.BasicQos(0, 1, false);
            //MessageReceiver messageReceiver = new MessageReceiver(model);
            //model.BasicConsume("CloadTest.UserInterfaceTaskManager", false, messageReceiver);
            //Console.ReadLine();

            // And call the response method
            string responseMethod = "BookTableResponse";
            string responseMessage = "Booking request received.";
            await Clients.All.SendAsync(responseMethod, responseMessage);
            //await Clients.Caller.SendAsync(responseMethod, responseMessage);
        }

        //public async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        //{
        //    var message = Encoding.UTF8.GetString(@event.Body);
        //    Console.WriteLine($"Begin processing {message}");
        //    //await Task.Delay(10000);
        //    Console.WriteLine($"End processing {message}");
        //    // And call the response method
        //    string responseMethod = "BookTableResponse";
        //    string responseMessage = "Booking request received.";
        //    await _hubContext.Clients.All.SendAsync(responseMethod, "A Message from Async Consumer + Async Callback");
        //    Console.WriteLine("Sent message to client...");
        //}
    }
}