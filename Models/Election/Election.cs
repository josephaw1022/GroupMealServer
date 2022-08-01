using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GroupMealApi.Models;


public class Election
{
    [StringLength(24, MinimumLength = 24)]
    public string GroupId { get; set; } = null!;

    [Range(0, 3)]
    public int? Rounds { get; set; } = null;

    [Range(0, 3)]
    public int? CurrentRound { get; set; } = null;

    public bool IsActive { get; set; } = false;

    public bool StoppedHalfway { get; set; } = false;

}