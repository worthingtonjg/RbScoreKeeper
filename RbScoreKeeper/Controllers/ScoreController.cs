using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RbScoreKeeper.Hubs;
using RbScoreKeeper.Models;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : Controller
    {
        private IHubContext<ScoreHub> _hubContext;
        private IAppState _appState;

        public ScoreController(IHubContext<ScoreHub> hubContext, IAppState appState)
        {
            _hubContext = hubContext;
            _appState = appState;
        }

        [HttpPut("{flicId}/increment")]
        public async void SinglePress(string flicId)
        {
            _appState.IncrementScore(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }

        [HttpPut("{flicId}/decrement")]
        public async void DoublePress(string flicId)
        {
            _appState.DecrementScore(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }

        [HttpDelete("{flicId}")]
        public async void LongPress(string flicId)
        {
            _appState.HandleLongPress(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }
    }
}
