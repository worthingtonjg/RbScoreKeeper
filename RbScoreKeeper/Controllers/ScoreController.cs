using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private static AppState app;
        private IHubContext<ScoreHub> _hubContext;
        private IAppState _appState;

        public ScoreController(IHubContext<ScoreHub> hubContext, IAppState appState)
        {
            _hubContext = hubContext;
            _appState = appState;
        }

        [HttpPut("{flicId}/increment")]
        public async void IncrementScore(string flicId)
        {
            _appState.IncrementScore(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }

        [HttpPut("{flicId}/decrement")]
        public async void DecrementScore(string flicId)
        {
            _appState.DecrementScore(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }

        [HttpDelete("{flicId}")]
        public async void NextGame(string flicId)
        {
            _appState.NextGame(flicId);
            await _hubContext.Clients.All.SendAsync("Refresh", flicId);
        }
    }
}
