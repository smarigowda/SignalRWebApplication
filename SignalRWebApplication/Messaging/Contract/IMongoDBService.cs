using MongoDB.Driver;
public interface IMongoDBService
{
    IMongoClient getClient();
}