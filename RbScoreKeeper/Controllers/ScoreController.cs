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
        private static Dictionary<string, FlicButtonBinding> _scores;
        private IHubContext<ScoreHub> _hubContext;

        public ScoreController(IHubContext<ScoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if(_scores == null)
            {
                _scores = new Dictionary<string, FlicButtonBinding>();
                _scores["001"] = new FlicButtonBinding("001", "Alan", 0);
                _scores["002"] = new FlicButtonBinding("002", "Jon", 0);
                _scores["003"] = new FlicButtonBinding("003", "Tony", 0);
            }

            return PartialView("ScoresPartial",_scores);
        }

        [HttpGet("{flicId}", Name = "Get")]
        public FlicButtonBinding Get(string flicId)
        {
            if (_scores.ContainsKey(flicId))
            {
                return _scores[flicId];
            }

            return null;
        }

        [HttpPut("{flicId}/assign")]
        public async void AssignFlic(string flicId, [FromQuery]string name)
        {
            if (_scores.ContainsKey(flicId))
            {
                _scores[flicId].PlayerName = name;
                await _hubContext.Clients.All.SendAsync("Refresh", flicId);
            }
        }

        [HttpPut("{flicId}/increment")]
        public async void IncrementScore(string flicId)
        {
            if (_scores.ContainsKey(flicId))
            {
                ++_scores[flicId].PlayerScore;
                await _hubContext.Clients.All.SendAsync("Refresh", flicId);
            }
        }

        [HttpPut("{flicId}/decrement")]
        public async void DecrementScore(string flicId)
        {
            if (_scores.ContainsKey(flicId))
            {
                --_scores[flicId].PlayerScore;
                await _hubContext.Clients.All.SendAsync("Refresh", flicId);
            }
        }

        [HttpDelete("{flicId}")]
        public async void ResetScore(string flicId)
        {
            if (_scores.ContainsKey(flicId))
            {
                _scores[flicId].PlayerScore = 0;
                await _hubContext.Clients.All.SendAsync("Refresh", flicId);
            }
        }
    }
}
