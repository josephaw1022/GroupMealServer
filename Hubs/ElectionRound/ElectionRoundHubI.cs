namespace GroupMealApi.AppHubs
{
    public interface ElectionRoundHubI
    {
        Task JoinElection(string electionId, string groupId);
    }
}