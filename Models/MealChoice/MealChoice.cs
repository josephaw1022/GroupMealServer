using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GroupMealApi.Models
{
    public class MealChoice
    {
        [Required]
        public string GroupId { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public string? ImageUrl { get; set; } = null;

        public string? MenuUrl { get; set; } = null;
    }
}