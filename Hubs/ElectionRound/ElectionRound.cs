using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GroupMealApi.Models;
using GroupMealApi.Services;



namespace GroupMealApi.AppHubs
{

    public class ElectionRoundHub : Hub<ElectionRoundHubI>
    {

        private readonly ILogger<ElectionRoundHub> _logger;
        private readonly ElectionRoundService _electionRoundService;

        public ElectionRoundHub(ElectionRoundService electionRoundService, ILogger<ElectionRoundHub> logger)
        {
            _electionRoundService = electionRoundService;
            _logger = logger;
        }

        public async Task JoinElection(string electionId, string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, electionId);
            await Clients.Group(electionId).JoinElection(electionId, groupId);
        }

    }
}