using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RbScoreKeeper.Hubs;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : Controller
    {
        private static Dictionary<string, int> _scores;
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
                _scores = new Dictionary<string, int>();
                _scores["Jon"] = 0;
                _scores["Tony"] = 0;
                _scores["Alan"] = 0;
            }

            return PartialView("Scores",_scores);
        }

        [HttpGet("{name}", Name = "Get")]
        public int? Get(string name)
        {
            if (_scores.ContainsKey(name))
            {
                return _scores[name];
            }

            return null;
        }

        [HttpPut("{name}/increment")]
        public async void IncrementScore(string name)
        {
            if (_scores.ContainsKey(name))
            {
                ++_scores[name];
                await _hubContext.Clients.All.SendAsync("Refresh", name);
            }
        }

        [HttpPut("{name}/decrement")]
        public async void DecrementScore(string name)
        {
            if (_scores.ContainsKey(name))
            {
                --_scores[name];
                await _hubContext.Clients.All.SendAsync("Refresh", name);
            }
        }

        [HttpDelete("{name}")]
        public async void ResetScore(string name)
        {
            if (_scores.ContainsKey(name))
            {
                _scores[name] = 0;
                await _hubContext.Clients.All.SendAsync("Refresh", name);
            }
        }
    }
}
