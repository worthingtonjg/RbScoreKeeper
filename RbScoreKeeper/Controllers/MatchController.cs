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
    public class MatchController : Controller
    {
        private IHubContext<ScoreHub> _hubContext;
        private IAppState _appState;
        
        public MatchController(IHubContext<ScoreHub> hubContext, IAppState appState)
        {
            _hubContext = hubContext;
            _appState = appState;
        }

        [HttpGet()]
        public Match GetMatch()
        {
            return _appState.GetActiveMatch();
        }

        [HttpGet("partial")]
        public IActionResult GetMatchPartial()
        {
            Match match = _appState.GetActiveMatch();

            return PartialView("MatchPartial", match);
        }

        [HttpPost("create")]
        public async void CreateMatch(Guid groupId)
        {
            _appState.CreateMatch(groupId);
            await _hubContext.Clients.All.SendAsync("Refresh");
        }

        [HttpPost("game/reset")]
        public async void RestCurrentGame()
        {
            _appState.RestartCurrentGame();
            await _hubContext.Clients.All.SendAsync("Refresh");
        }

        [HttpPost("game/next")]
        public async void NextGame()
        {
            _appState.NextGame();
            await _hubContext.Clients.All.SendAsync("Refresh");
        }

        [HttpPost("cancel")]
        public async void CancelMatch()
        {
            _appState.CancelMatch();
            await _hubContext.Clients.All.SendAsync("Refresh");
        }

        [HttpPost("end")]
        public async void EndMatch()
        {
            _appState.EndMatch();
            await _hubContext.Clients.All.SendAsync("Refresh");
        }

        [HttpPost("history")]
        public void History()
        {
            _appState.GetMatchHistory();
        }
    }
}