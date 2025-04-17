using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlinkIt.Repository.Models;

public class User
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("MobileNumber")]
    public string MobileNumber { get; set; }

    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("Addresses")]
    public List<string> Addresses { get; set; }
}