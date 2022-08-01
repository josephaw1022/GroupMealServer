using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace GroupMealApi.Models;


public class AccountJoinGroup
{

    [Key]
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    [Phone]
    public string PhoneNumber { get; set; } = null!;


    [BsonElement("GroupId")]
    [JsonPropertyName("GroupId")]
    public string GroupId { get; set; } = null!;
}