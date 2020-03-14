using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRTableBooking.Hubs;

public class RabbitMQChannelService : IRabbitMQChannelService
{
    protected readonly ConnectionFactory factory;
    protected readonly IConnection connection;
    protected readonly IModel channel;

    public RabbitMQChannelService()
    {
        // Open connection to RabbitMQ and create a channel
        factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
    }

    public virtual IModel getChannel()
    {
        return channel;
    }
}