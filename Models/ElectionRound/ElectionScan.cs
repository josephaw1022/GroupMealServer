namespace GroupMealApi.Models
{
    public class ElectionRoundScan
    {
        public string? ElectionId { get; set; }

        public string? GroupId { get; set; }

        public string? Winner { get; set; }

        public int? Rounds { get; set; }
    }
}