using System;

namespace OxfordParser.Data
{
    public class MongoOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
        public bool DropDatabase { get; set; }
    }
}
