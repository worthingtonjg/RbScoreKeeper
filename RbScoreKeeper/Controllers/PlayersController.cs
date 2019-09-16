using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;
using System;
using System.Collections.Generic;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IAppState _appState;

        public PlayersController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public List<Player> GetPlayers()
        {
            return _appState.GetPlayers();
        }

        [HttpPost()]
        public void AddPlayer([FromQuery]string name)
        {
            _appState.AddPlayer(name);
        }

        [HttpDelete("{playerId}")]
        public void DeletePlayer(Guid playerId)
        {
            _appState.DeletePlayer(playerId);
        }

    }
}