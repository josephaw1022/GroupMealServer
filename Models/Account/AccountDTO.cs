using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


namespace GroupMealApi.Models;



///  <summary>
///  The account class that is used as for getting a account or list of accounts and for updating a account.
/// </summary>
public class AccountDTO : Account
{
    [BsonId]
    [BsonElement("_id")]
    [JsonPropertyName("Id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();

}