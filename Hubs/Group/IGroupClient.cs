namespace GroupMealApi.AppHubs
{
    public interface IGroupClient
    {
        Task GetGroupCount(int groupCount);

        Task JoinRoom(string roomId);

        Task LeaveRoom(string roomId);
    }
}