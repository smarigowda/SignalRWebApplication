using RabbitMQ.Client;

public interface IRabbitMQChannelService
{
    IModel getChannel();
}