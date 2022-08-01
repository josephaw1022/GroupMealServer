using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace GroupMealApi.Models;

[BsonIgnoreExtraElements]
[BsonDiscriminator("Election")]
public class ElectionDBO : Election, IBase
{
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();


    [BsonElement("CreatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; } = DateTime.UtcNow;


    [BsonElement("UpdatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    [BsonElement("Deleted")]
    public bool Deleted { get; set; } = false;
}