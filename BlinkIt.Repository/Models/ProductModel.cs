using MongoDB.Bson.Serialization.Attributes;

namespace BlinkIt.Repository.Models;

public class Product
    {
        [BsonId]
        public Guid _id { get; set; }
        public string product_name { get; set; }
        public Guid category_id { get; set; }
        public int price { get; set; }
        public int stock { get; set; }
        public string unit { get; set; }
        public string photo_url { get; set; }

        public string discountedPrice { get; set; }

        public string SellerName { get; set; }

    }
