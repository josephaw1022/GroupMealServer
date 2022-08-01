using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GroupMealApi.Models
{
    public class ElectionRound
    {

        [Required]
        public string ElectionId { get; set; } = null!;

        [Required]
        public string GroupId { get; set; } = null!;

        public string? Winner { get; set; }

        public int? Rounds { get; set; }
    }
}