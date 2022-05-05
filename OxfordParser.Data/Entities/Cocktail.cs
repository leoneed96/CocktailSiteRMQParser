using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxfordParser.Data.Entities
{
    public class Cocktail
    {
        [BsonId]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string Title { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool Processed { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        public bool Processing { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string RelativeDetailsUrl { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string About { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string ImageUrl { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public List<Stuff> Stuffs { get; set; } = new List<Stuff>();

        public List<string> Receipt { get; set; } = new List<string>();


    } 
    
}
