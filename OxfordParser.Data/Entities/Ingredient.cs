using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OxfordParser.Data.Entities
{
    public class Ingredient
    {
        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

        public double Amount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string Units { get; set; }
    }
}
