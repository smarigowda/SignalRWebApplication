using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRTableBooking.Hubs;

public class RabbitMQService : IRabbitMQService
{
    protected readonly ConnectionFactory _factory;
    protected readonly IConnection _connection;
    protected readonly IModel _channel;

    protected readonly IServiceProvider _serviceProvider;

    public RabbitMQService(IServiceProvider serviceProvider)
    {
        // Opens the connections to RabbitMQ
        //_factory = new ConnectionFactory() { HostName = "localhost" };
        //_connection = _factory.CreateConnection();
        //_channel = _connection.CreateModel();

        _serviceProvider = serviceProvider;
        var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
        _channel = channelService.getChannel();
    }

    public virtual void Connect()
    {
        // Declare a RabbitMQ Queue
        _channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(_channel);

        // When we receive a message from SignalR
        consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
        {
            string message = Encoding.UTF8.GetString(ea.Body, 0, ea.Body.Length);
            // Get the TableBookingHub from SignalR (using DI)
            var chatHub = (IHubContext<TableBookingHub>)_serviceProvider.GetService(typeof(IHubContext<TableBookingHub>));
            // Send message to all users in SignalR
            chatHub.Clients.All.SendAsync("BookTableResponse", message);
        };

        // Consume a RabbitMQ Queue
        _channel.BasicConsume(queue: "TestQueue", autoAck: true, consumer: consumer);
    }
}