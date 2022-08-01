using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace GroupMealApi.Models;


/// <summary>
/// This is the account class that is stored in the database 
/// </summary>
[BsonDiscriminator("Account")]
[BsonIgnoreExtraElements]
public class AccountDBO : Account, IBase
{
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();


    [BsonElement("CreatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; } = DateTime.Now;


    [BsonElement("UpdatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    [BsonElement("Deleted")]
    public bool Deleted { get; set; } = false;
}
