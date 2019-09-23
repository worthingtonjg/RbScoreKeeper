﻿using Microsoft.AspNetCore.Mvc;
using RbScoreKeeper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RbScoreKeeper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : Controller
    {
        private IAppState _appState;

        public PlayersController(IAppState appState)
        {
            _appState = appState;
        }

        [HttpGet()]
        public async Task<List<Player>> GetPlayers()
        {
            return await _appState.GetPlayersAsync();
        }

        [HttpPost()]
        public async Task AddPlayer([FromQuery]string name)
        {
            await _appState.AddPlayerAsync(name);
        }

        [HttpDelete("{playerId}")]
        public async Task DeletePlayer(Guid playerId)
        {
            await _appState.DeletePlayerAsync(playerId);
        }

    }
}