using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OxfordParser.Data.Entities
{
    public class Stuff
    {
        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

        public double Amount { get; set; }
        public string Units { get; set; }
    }
}
