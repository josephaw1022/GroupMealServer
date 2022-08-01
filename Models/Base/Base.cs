using MongoDB.Bson;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;



namespace GroupMealApi.Models;


/// <summary>
/// This is to be implemented in every mongo db collection class
/// </summary>
interface IBase
{

    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    string Id { get; init; }


    [BsonElement("CreatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    DateTime CreatedAt { get; }


    [BsonElement("UpdatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    DateTime UpdatedAt { get; set; }


    [BsonElement("Deleted")]
    [JsonPropertyName("deleted")]
    bool Deleted { get; set; }
}
