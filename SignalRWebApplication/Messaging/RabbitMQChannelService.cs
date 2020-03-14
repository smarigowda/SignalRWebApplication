using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRTableBooking.Hubs;

public class RabbitMQChannelService : IRabbitMQChannelService
{
    protected readonly ConnectionFactory _factory;
    protected readonly IConnection _connection;
    protected readonly IModel _channel;

    protected readonly IServiceProvider _serviceProvider;

    public RabbitMQChannelService(IServiceProvider serviceProvider)
    {
        // Opens the connections to RabbitMQ
        _factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _serviceProvider = serviceProvider;
    }

    public virtual IModel CreateChannel()
    {
        return _channel;
    }
}