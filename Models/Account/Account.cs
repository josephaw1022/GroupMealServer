using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GroupMealApi.Models;


/// <summary>
///  This is the account class that is used to create a new account
/// </summary>
public class Account
{
    [Required]
    [BsonElement("FirstName")]
    public string FirstName { get; set; } = null!;

    [Required]
    [BsonElement("LastName")]
    public string LastName { get; set; } = null!;

    [Required]
    [BsonElement("Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = null!;

    [Required]
    [BsonElement("PhoneNumber")]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = null!;

    public string? GroupId { get; set; }

}