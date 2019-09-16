using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private IAppState _appState;

        public MatchesController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public List<Match> GetMatches()
        {
            return _appState.GetActiveMatches();
        }

        [HttpPost("create")]
        public void CreateMatch(Guid groupId)
        {
            _appState.CreateMatch(groupId);
        }

        [HttpPost("{matchId}/reset")]
        public void RestCurrentGame(Guid matchId)
        {
            _appState.RestartCurrentGame(matchId);
        }

        [HttpPost("{matchId}/next")]
        public void NextGame(Guid matchId)
        {
            _appState.NextGame(matchId);
        }

        [HttpPost("{matchId}/cancel")]
        public void CancelMatch(Guid matchId)
        {
            _appState.CancelMatch(matchId);
        }

        [HttpPost("{matchId}/end")]
        public void EndMatch(Guid matchId)
        {
            _appState.EndMatch(matchId);
        }

    }
}