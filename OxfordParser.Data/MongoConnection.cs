using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace OxfordParser.Data
{
    public class MongoConnection
    {
        public IMongoDatabase MongoDatabase { get; }
        public MongoConnection(IOptions<MongoOptions> options)
        {
            var optionsValue = options.Value;

            var client = new MongoClient(optionsValue.ConnectionString);
            if (optionsValue.DropDatabase)
                client.DropDatabase(optionsValue.DatabaseName);
            var database = client.GetDatabase(optionsValue.DatabaseName);

            MongoDatabase = database;
        }

        public IMongoQueryable<T> GetQuery<T>() => MongoDatabase.GetCollection<T>(typeof(T).Name).AsQueryable();
        public IMongoCollection<T> GetCollection<T>() 
        {
            var collectionName = typeof(T).Name;
            var c = MongoDatabase.GetCollection<T>(collectionName);
            if (c == null)
            {
                MongoDatabase.CreateCollection(collectionName);
                return MongoDatabase.GetCollection<T>(collectionName);
            }
            else
                return c;
                
        }
    }
}
