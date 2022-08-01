using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GroupMealApi.Models;

public class ElectionScan
{
    [Range(0, 3)]
    public int? Rounds { get; set; } 

    [Range(0,3)]
    public int? CurrentRound { get; set; } 

    public bool? IsActive { get; set; } 

    public bool? StoppedHalfway { get; set; }

    [StringLength(24)]
    public string? GroupId { get; set; } = "";
}
