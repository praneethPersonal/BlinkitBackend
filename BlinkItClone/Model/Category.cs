using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Libmongocrypt;

namespace BlinkItClone.Model
{
    public class Category
    {
        [BsonId]
        public Guid _id { get; set; }
        public string category_name { get; set; }
        public string description { get; set; }
    }
}
