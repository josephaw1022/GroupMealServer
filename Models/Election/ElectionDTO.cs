using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace GroupMealApi.Models;

public class ElectionDTO : Election
{
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
}