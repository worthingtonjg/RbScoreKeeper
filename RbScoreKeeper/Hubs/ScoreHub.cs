using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RbScoreKeeper.Hubs
{
    public class ScoreHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
