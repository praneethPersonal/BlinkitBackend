using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlinkItClone.Model
{
    public class Cart
    {
        [BsonId]
        public string _id { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("ProductId")]
        public string ProductId { get; set; }
        [BsonElement("Quantity")]
        public int Quantity { get; set; }
    }
}
