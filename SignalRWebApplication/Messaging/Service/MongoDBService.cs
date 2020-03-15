using System;
using MongoDB.Driver;

namespace SignalRWebApplication.Messaging.Contract
{
    public class MongoDBService : IMongoDBService
    {
        protected readonly IMongoClient dbClient;
        public MongoDBService()
        {
            dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases are:");

            foreach (var item in dbList)
            {
                Console.WriteLine(item);
            }
        }
        public IMongoClient getClient()
        {
            return dbClient;
        }
    }
}