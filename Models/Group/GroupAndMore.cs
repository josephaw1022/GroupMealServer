namespace GroupMealApi.Models
{
    public class GroupAndMoreDBO : GroupDBO
    {
        public IEnumerable<AccountDBO> GroupAccounts { get; set; } = new List<AccountDBO>();
        public IEnumerable<MealChoiceDBO> GroupMealChoices { get; set; } = new List<MealChoiceDBO>();
    }

    public class GroupAndMoreDTO : GroupDTO
    {
        public IEnumerable<AccountJoinGroup> GroupAccounts { get; set; } = new List<AccountJoinGroup>();
        public IEnumerable<MealChoiceJoinGroup> GroupMealChoices { get; set; } = new List<MealChoiceJoinGroup>();
    }
}