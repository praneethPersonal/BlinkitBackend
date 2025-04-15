using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Net;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("MobileNumber")]
    public string MobileNumber { get; set; }

    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("ProductsBought")]
    public List<string> ProductsBought { get; set; }

    [BsonElement("Addresses")]
    public List<string> Addresses { get; set; }
}


public class Seller
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("userName")]
    public string UserName { get; set; }


    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("Products")]
    public List<string> Products { get; set; }


}
public class LoginRequest
{
    public string MobileNumber { get; set; }
    public string Password { get; set; }
}
public class AddProductsRequest
{
    public List<string> Products { get; set; }
}

public class AddProductsRequestBySeller
{
    public string Products { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
public class SellerRequest
{
    public string SellerName { get; set; }
}
public class UpdateStockRequest
{
    public string Product_name { get; set; }
    public int NewStock { get; set; }
}