using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRTableBooking.Hubs;

public class RabbitMQService : IRabbitMQService
{
    protected readonly ConnectionFactory _factory;
    protected readonly IConnection _connection;
    protected readonly IModel channel;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly string TableBooking;

    public RabbitMQService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
        channel = channelService.getChannel();

        string qname = configuration.GetValue<string>("RabbitMQNames:TableBooking");
        TableBooking = qname;
    }

    public virtual void Connect()
    {
        // Declare a RabbitMQ Queue
        channel.QueueDeclare(queue: TableBooking, durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(channel);

        // When we receive a message from SignalR
        consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
        {
            string message = Encoding.UTF8.GetString(ea.Body, 0, ea.Body.Length);
            // Get TableBookingHub from SignalR (using DI)
            var chatHub = (IHubContext<TableBookingHub>)_serviceProvider.GetService(typeof(IHubContext<TableBookingHub>));
            // Send message to all users in SignalR
            chatHub.Clients.All.SendAsync("BookTableResponse", message);
        };

        // Consume a RabbitMQ Queue.
        // You should be able to see a consumer in RabbitMQ admin console.
        channel.BasicConsume(queue: TableBooking, autoAck: true, consumer: consumer);
    }
}