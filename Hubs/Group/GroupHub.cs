using Microsoft.AspNetCore.SignalR;
using GroupMealApi.Services;


namespace GroupMealApi.AppHubs
{
    public class GroupHub : Hub<IGroupClient>
    {
        private readonly GroupService groupService; // add the group service to the constructor

        public GroupHub(GroupService groupService)
        {
            this.groupService = groupService;
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }


        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }


        public async Task RequestGroupCount()
        {
            var groupLength = groupService.Count();

            await Clients.All.GetGroupCount(groupLength);
        }
    }
}