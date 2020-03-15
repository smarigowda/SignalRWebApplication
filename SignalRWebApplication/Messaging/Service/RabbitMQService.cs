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
    protected string queueName;

    public RabbitMQService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        var channelService = (IRabbitMQChannelService)serviceProvider.GetService(typeof(IRabbitMQChannelService));
        channel = channelService.getChannel();

        queueName = configuration.GetValue<string>("RabbitMQNames:SignalRWeb");
    }

    public virtual void Connect()
    {

        string exchangeName = "demoexchange";
        string keyBindingName = "directexchange_key";

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queueName, exchangeName, keyBindingName);
        var consumer = new EventingBasicConsumer(channel);
        receiveMessageFromSignalR(consumer);
    }

    public void receiveMessageFromSignalR(EventingBasicConsumer consumer)
    {
        consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
        {
            string message = Encoding.UTF8.GetString(ea.Body, 0, ea.Body.Length);
            // using DI
            var chatHub = (IHubContext<TableBookingHub>)_serviceProvider.GetService(typeof(IHubContext<TableBookingHub>));
            chatHub.Clients.All.SendAsync("BookTableResponse", message);
        };
        // You should be able to see a consumer in RabbitMQ admin console.
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }
}