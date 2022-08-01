using Microsoft.AspNetCore.SignalR;


namespace GroupMealApi.AppHubs;




public interface ElectionHubClient
{
    Task JoinElection(string electionId, string groupId);

    Task LeaveElection(string electionId, string groupId);

    Task Vote(string electionId, string groupId, string voterId, string candidateId);
}

public class ElectionHub :  Hub<ElectionHubClient>
{

    public async Task JoinElection(string electionId, string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, electionId);
        await Clients.Group(electionId).JoinElection(electionId, groupId);
    }


    public async Task LeaveElection(string electionId, string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, electionId);
        await Clients.Group(electionId).LeaveElection(electionId, groupId);
    }


    public async Task Vote(string electionId, string groupId, string voterId, string candidateId)
    {
        await Clients.Group(electionId).Vote(electionId, groupId, voterId, candidateId);
    }

    

}